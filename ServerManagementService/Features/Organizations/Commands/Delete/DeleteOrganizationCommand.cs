using Common.Core.CQRS.Commands;

namespace ServerManagementService.Features.Organizations.Commands.Delete
{
    public class DeleteOrganizationCommand : Command<bool>
    {
        public int OrganizationId { get; set; }
    }
}
