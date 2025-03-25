using Common.Core.CQRS.Commands;

namespace ServerManagementService.Features.Auth.Commands.Register
{
    public class RegisterCommand : Command<AuthResponse>
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int OrganizationId { get; set; }
    }
}
