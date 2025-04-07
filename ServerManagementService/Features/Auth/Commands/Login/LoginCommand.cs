using Common.Core.CQRS.Commands;
using ServerManagementService.Controllers;

namespace ServerManagementService.Features.Auth.Commands.Login
{
    public class LoginCommand : Command<AuthResponse>
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
