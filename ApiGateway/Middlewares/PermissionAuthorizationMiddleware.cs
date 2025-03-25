using Common.Core.Auth;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Middlewares
{
    public class PermissionAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpClientFactory _httpClientFactory;

        public PermissionAuthorizationMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory)
        {
            _next = next;
            _httpClientFactory = httpClientFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Exclude auth endpoints from permission checks
            if (context.Request.Path.StartsWithSegments("/api/auth") ||
                context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            // 1. Get the user ID from context (set by JwtMiddleware)
            if (!context.Items.TryGetValue("UserId", out var userIdObj) || userIdObj == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Token missing or invalid");
                return;
            }

            var userId = (int)userIdObj;

            // 2. Extract resource type and action from the request
            var path = context.Request.Path.Value?.ToLower();
            var method = context.Request.Method;

            var resourceType = ExtractResourceTypeFromPath(path);
            var action = ConvertMethodToAction(method);

            // 3. Check permission against the ServerManagementService
            bool hasPermission = await CheckPermissionAsync(userId, resourceType, action);

            if (!hasPermission)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync($"Forbidden: You don't have permission to {action} {resourceType}");
                return;
            }

            // 4. Check organization access (if applicable)
            int? organizationId = ExtractOrganizationIdFromRequest(context);

            if (organizationId.HasValue && resourceType != ResourceType.Organization)
            {
                bool hasOrganizationAccess = await CheckOrganizationAccessAsync(userId, organizationId.Value);

                if (!hasOrganizationAccess)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync($"Forbidden: You don't have access to organization with ID {organizationId}");
                    return;
                }
            }

            await _next(context);
        }

        private ResourceType ExtractResourceTypeFromPath(string path)
        {
            // Örnek: "/api/products" -> ResourceType.Product
            var segments = path?.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments?.Length >= 2 && segments[0] == "api")
            {
                // API yolundan resource type'ı belirle
                var resource = segments[1].ToLower();
                // Çoğul -> tekil dönüşümü ve enum değerine çevirme
                if (resource.EndsWith("s"))
                    resource = resource.Substring(0, resource.Length - 1);

                // Enum değerine çevirme
                if (Enum.TryParse(typeof(ResourceType), resource, true, out var resourceType))
                    return (ResourceType)resourceType;
            }

            return ResourceType.User; // Varsayılan olarak User
        }

        private ActionType ConvertMethodToAction(string method)
        {
            return method.ToUpper() switch
            {
                "GET" => ActionType.Read,
                "POST" => ActionType.Create,
                "PUT" => ActionType.Update,
                "DELETE" => ActionType.Delete,
                _ => ActionType.Read
            };
        }

        private int? ExtractOrganizationIdFromRequest(HttpContext context)
        {
            // 1. Try to get from route values
            if (context.Request.RouteValues.TryGetValue("organizationId", out var routeOrgId) &&
                int.TryParse(routeOrgId?.ToString(), out int orgId))
            {
                return orgId;
            }

            // 2. Try to get from query string
            if (context.Request.Query.TryGetValue("organizationId", out var queryOrgId) &&
                int.TryParse(queryOrgId.FirstOrDefault(), out orgId))
            {
                return orgId;
            }

            // 3. Try to get from headers
            if (context.Request.Headers.TryGetValue("X-Organization-Id", out var headerOrgId) &&
                int.TryParse(headerOrgId.FirstOrDefault(), out orgId))
            {
                return orgId;
            }

            // 4. If no organization ID is specified, use default from token
            if (context.Items.TryGetValue("DefaultOrganizationId", out var defaultOrgId) &&
                defaultOrgId != null)
            {
                return (int)defaultOrgId;
            }

            return null;
        }

        private async Task<bool> CheckPermissionAsync(int userId, ResourceType resourceType, ActionType action)
        {
            using var httpClient = _httpClientFactory.CreateClient("ServerManagementAPI");

            var checkRequest = new PermissionCheckRequest
            {
                UserId = userId,
                ResourceType = resourceType,
                Action = action
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(checkRequest),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync("api/permissions/check", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<PermissionCheckResponse>(responseContent);
                return result?.HasPermission ?? false;
            }

            return false;
        }

        private async Task<bool> CheckOrganizationAccessAsync(int userId, int organizationId)
        {
            using var httpClient = _httpClientFactory.CreateClient("ServerManagementAPI");

            var checkRequest = new OrganizationCheckRequest
            {
                UserId = userId,
                OrganizationId = organizationId
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(checkRequest),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync("api/organizations/check-access", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<OrganizationCheckResponse>(responseContent);
                return result?.HasAccess ?? false;
            }

            return false;
        }
    }
}