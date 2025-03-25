using Common.Core.CQRS.Commands;
using ServerManagementService.Domain.Entities;

namespace ServerManagementService.Features.Organizations.Commands.Update
{
    public class UpdateOrganizationCommand : Command<bool>
    {
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;
    }
}
