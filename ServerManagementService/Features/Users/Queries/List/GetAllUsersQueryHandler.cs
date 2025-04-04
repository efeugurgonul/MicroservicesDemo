using Common.Core.CQRS.Queries;
using Microsoft.EntityFrameworkCore;
using ServerManagementService.Data;
using ServerManagementService.Domain.Entities;

namespace ServerManagementService.Features.Users.Queries.List
{
    public class GetAllUsersQueryHandler : QueryHandler<GetAllUsersQuery, List<User>>
    {
        private readonly ServerManDbContext _dbContext;

        public GetAllUsersQueryHandler(ServerManDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<List<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Users
                .Include(u => u.Organizations)
                .AsQueryable();

            if (request.Filter.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.Filter.IsActive.Value);
            }

            if (request.Filter.OrganizationId.HasValue)
            {
                query = query.Where(u => u.Organizations.Any(uo => uo.OrganizationId == request.Filter.OrganizationId.Value));
            }

            return await query.ToListAsync(cancellationToken);
        }
    }
}