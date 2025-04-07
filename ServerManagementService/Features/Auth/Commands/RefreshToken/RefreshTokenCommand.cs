using Common.Core.CQRS.Commands;
using ServerManagementService.Controllers;

namespace ServerManagementService.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand : Command<AuthResponse>
    {
        public string RefreshToken { get; set; } = null!;
    }
}
