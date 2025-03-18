using Common.Core.CQRS.Queries;
using ServerManagentService.Domain.Entities;

namespace ServerManagentService.Features.Organizations.Queries.Detail
{
    public class GetOrganizationByIdQuery : Query<Organization>
    {
        public int OrganizationId { get; set; }
    }
}
