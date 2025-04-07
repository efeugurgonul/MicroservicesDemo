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

        public PermissionAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
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

            // Check if user is authenticated
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Not authenticated");
                return;
            }

            // Extract resource type and action from the request
            var path = context.Request.Path.Value?.ToLower();
            var method = context.Request.Method;

            var resourceType = ExtractResourceTypeFromPath(path);
            var action = ConvertMethodToAction(method);

            // Get all claims
            var claims = context.User.Claims;

            // Check permission from claims
            bool hasPermission = PermissionHelper.CheckPermissionFromClaims(claims, resourceType, action);

            if (!hasPermission)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync($"Forbidden: You don't have permission to {action} {resourceType}");
                return;
            }

            // Check organization access if needed
            int? organizationId = ExtractOrganizationIdFromRequest(context);

            if (organizationId.HasValue && resourceType != ResourceType.Organization)
            {
                bool hasOrganizationAccess = PermissionHelper.CheckOrganizationAccessFromClaims(claims, organizationId.Value);

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
    }
}