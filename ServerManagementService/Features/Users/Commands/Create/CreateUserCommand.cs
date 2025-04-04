using Common.Core.CQRS.Commands;

namespace ServerManagementService.Features.Users.Commands.Create
{
    public class CreateUserCommand : Command<int>
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int DefaultOrganizationId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}