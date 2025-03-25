using Common.Core.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using ServerManagementService.Data;

namespace ServerManagementService.Features.Organizations.Commands.Delete
{
    public class DeleteOrganizationCommandHandler : CommandHandler<DeleteOrganizationCommand, bool>
    {
        private readonly ServerManDbContext _dbContext;
        public DeleteOrganizationCommandHandler(ServerManDbContext context)
        {
            _dbContext = context;
        }

        public override async Task<bool> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await _dbContext.Organizations.FirstOrDefaultAsync(o => o.Id == request.OrganizationId, cancellationToken);

            if(organization == null)
                return false;

            _dbContext.Organizations.Remove(organization);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
