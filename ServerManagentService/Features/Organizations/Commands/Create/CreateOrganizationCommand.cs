using Common.Core.CQRS.Commands;
using ServerManagentService.Domain.Entities;

namespace ServerManagentService.Features.Organizations.Commands.Create
{
    public class CreateOrganizationCommand : Command<int>
    {
        public Organization Organization { get; set; } = null!;
    }
}
