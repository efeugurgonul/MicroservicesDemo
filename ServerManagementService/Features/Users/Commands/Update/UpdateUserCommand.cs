using Common.Core.CQRS.Commands;

namespace ServerManagementService.Features.Users.Commands.Update
{
    public class UpdateUserCommand : Command<bool>
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? DefaultOrganizationId { get; set; }
        public bool? IsActive { get; set; }
    }
}