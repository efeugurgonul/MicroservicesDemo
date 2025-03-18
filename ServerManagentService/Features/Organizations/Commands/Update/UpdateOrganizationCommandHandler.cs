using Common.Core.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using ServerManagentService.Data;

namespace ServerManagentService.Features.Organizations.Commands.Update
{
    public class UpdateOrganizationCommandHandler : CommandHandler<UpdateOrganizationCommand, bool>
    {
        private readonly ServerManDbContext _dbContext;
        public UpdateOrganizationCommandHandler(ServerManDbContext context)
        {
            _dbContext = context;
        }

        public override async Task<bool> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await _dbContext.Organizations.FirstOrDefaultAsync(o => o.Id == request.OrganizationId, cancellationToken);

            if (organization == null)
                return false;

            organization.Name = request.Organization.Name;
            organization.Description = request.Organization.Description;
            organization.ActiveStatus = request.Organization.ActiveStatus;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
