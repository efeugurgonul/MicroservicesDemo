using Common.Core.CQRS.Queries;
using Microsoft.EntityFrameworkCore;
using ServerManagementService.Data;
using ServerManagementService.Domain.Entities;

namespace ServerManagementService.Features.Organizations.Queries.List
{
    public class GetAllOrganizationsQueryHandler : QueryHandler<GetAllOrganizationsQuery, List<Organization>>
    {
        private readonly ServerManDbContext _dbContext;
        public GetAllOrganizationsQueryHandler(ServerManDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<List<Organization>> Handle(GetAllOrganizationsQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Organizations.AsQueryable();

            if (request.Filter.ActiveStatus.HasValue)
            {
                query = query.Where(o => o.ActiveStatus == request.Filter.ActiveStatus.Value);
            }

            return await query.Select(p => new Organization
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ActiveStatus = p.ActiveStatus

            }).ToListAsync(cancellationToken);
        }
    }
}
