using Common.Core.CQRS.Queries;
using ServerManagementService.Domain.Entities;

namespace ServerManagementService.Features.Users.Queries.List
{
    public class UserFilter
    {
        public bool? IsActive { get; set; }
        public int? OrganizationId { get; set; }
    }

    public class GetAllUsersQuery : Query<List<User>>
    {
        public UserFilter Filter { get; set; } = new UserFilter();
    }
}