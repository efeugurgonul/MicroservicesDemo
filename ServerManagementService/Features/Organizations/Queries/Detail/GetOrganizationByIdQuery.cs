using Common.Core.CQRS.Queries;
using ServerManagementService.Domain.Entities;

namespace ServerManagementService.Features.Organizations.Queries.Detail
{
    public class GetOrganizationByIdQuery : Query<Organization>
    {
        public int OrganizationId { get; set; }
    }
}
