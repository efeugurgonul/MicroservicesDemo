using Common.Core.CQRS.Commands;
using ServerManagentService.Data;
using ServerManagentService.Domain.Entities;

namespace ServerManagentService.Features.Organizations.Commands.Create
{
    public class CreateOrganizationCommandHandler : CommandHandler<CreateOrganizationCommand, int>
    {
        private readonly ServerManDbContext _dBContext;

        public CreateOrganizationCommandHandler(ServerManDbContext dbContext)
        {
            _dBContext = dbContext;
        }

        public override async Task<int> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = new Organization
            {
                Name = request.Organization.Name,
                Description = request.Organization.Description,
                ActiveStatus = request.Organization.ActiveStatus
            };

            _dBContext.Organizations.Add(organization);
            await _dBContext.SaveChangesAsync(cancellationToken);

            return organization.Id;
        }
    }
}
