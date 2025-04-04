using Common.Core.CQRS.Queries;
using Microsoft.EntityFrameworkCore;
using ServerManagementService.Data;
using ServerManagementService.Domain.Entities;

namespace ServerManagementService.Features.Users.Queries.Detail
{
    public class GetUserByIdQueryHandler : QueryHandler<GetUserByIdQuery, User>
    {
        private readonly ServerManDbContext _dbContext;

        public GetUserByIdQueryHandler(ServerManDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Include(u => u.Organizations)
                .Include(u => u.Permissions)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            return user;
        }
    }
}