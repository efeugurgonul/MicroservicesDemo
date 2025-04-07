using Common.Core.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerManagementService.Services;
using System.Security.Claims;

namespace ServerManagementService.Controllers
{
    [ApiController]
    [Route("api/permissions")]
    [Authorize]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpPost("check")]
        public async Task<ActionResult<PermissionCheckResponse>> CheckPermission(PermissionCheckRequest request)
        {
            var hasPermission = await _permissionService.UserHasPermissionAsync(
                request.UserId, request.Action, request.ResourceType);

            return Ok(new PermissionCheckResponse { HasPermission = hasPermission });
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<ActionResult<List<Permission>>> GetUserPermissions(int userId)
        {
            // Sadece kendi izinlerini veya yönetici izni olan kullanıcılar görebilir
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (currentUserId != userId)
            {
                var hasManagePermission = await _permissionService.UserHasPermissionAsync(
                    currentUserId, Permission.ManageUserPermissions);

                if (!hasManagePermission)
                    return Forbid();
            }

            var permissions = await _permissionService.GetUserPermissionsAsync(userId);
            return Ok(permissions);
        }
    }

    [ApiController]
    [Route("api/organizations")]
    [Authorize]
    public class OrganizationAccessController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationAccessController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpPost("check-access")]
        public async Task<ActionResult<OrganizationCheckResponse>> CheckOrganizationAccess(OrganizationCheckRequest request)
        {
            var hasAccess = await _organizationService.UserHasOrganizationAccessAsync(
                request.UserId, request.OrganizationId);

            return Ok(new OrganizationCheckResponse { HasAccess = hasAccess });
        }
    }
}
