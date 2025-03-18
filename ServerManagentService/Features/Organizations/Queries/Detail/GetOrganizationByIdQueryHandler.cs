using Common.Core.CQRS.Queries;
using Microsoft.EntityFrameworkCore;
using ServerManagentService.Data;
using ServerManagentService.Domain.Entities;

namespace ServerManagentService.Features.Organizations.Queries.Detail
{
    public class GetOrganizationByIdQueryHandler : QueryHandler<GetOrganizationByIdQuery, Organization>
    {
        private readonly ServerManDbContext _dbContext;
        public GetOrganizationByIdQueryHandler(ServerManDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<Organization> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
        {
            var organization = await _dbContext.Organizations.FirstOrDefaultAsync(o => o.Id == request.OrganizationId, cancellationToken);

            if(organization == null)
                return null;

            return new Organization
            {
                Name = organization.Name,
                Description = organization.Description,
                ActiveStatus = organization.ActiveStatus
            };
        }
    }
}
