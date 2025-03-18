using Common.Core.CQRS.Commands;
using ServerManagentService.Domain.Entities;

namespace ServerManagentService.Features.Organizations.Commands.Update
{
    public class UpdateOrganizationCommand : Command<bool>
    {
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;
    }
}
