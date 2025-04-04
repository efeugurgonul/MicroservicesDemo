using Common.Core.CQRS.Queries;
using ServerManagementService.Domain.Entities;

namespace ServerManagementService.Features.Users.Queries.Detail
{
    public class GetUserByIdQuery : Query<User>
    {
        public int UserId { get; set; }
    }
}