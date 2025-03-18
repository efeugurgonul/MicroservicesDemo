using Common.Core.CQRS.Commands;

namespace ServerManagentService.Features.Organizations.Commands.Delete
{
    public class DeleteOrganizationCommand : Command<bool>
    {
        public int OrganizationId { get; set; }
    }
}
