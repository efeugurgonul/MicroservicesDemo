using Common.Core.CQRS.Queries;
using ServerManagementService.Domain.Entities;

namespace ServerManagementService.Features.Organizations.Queries.List
{
    public class OrganizationFilter
    {
        public int? ActiveStatus { get; set; }        
    }

    public class GetAllOrganizationsQuery : Query<List<Organization>>
    {
        public OrganizationFilter Filter { get; set; } = new OrganizationFilter();
    }
}
