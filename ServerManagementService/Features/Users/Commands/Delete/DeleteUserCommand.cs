using Common.Core.CQRS.Commands;

namespace ServerManagementService.Features.Users.Commands.Delete
{
    public class DeleteUserCommand : Command<bool>
    {
        public int UserId { get; set; }
    }
}