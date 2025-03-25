using Common.Core.CQRS.Commands;
using ServerManagementService.Domain.Entities;

namespace ServerManagementService.Features.Organizations.Commands.Create
{
    public class CreateOrganizationCommand : Command<int>
    {
        public Organization Organization { get; set; } = null!;
    }
}
