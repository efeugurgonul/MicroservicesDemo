using Common.Core.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using ServerManagementService.Data;

namespace ServerManagementService.Features.Users.Commands.Delete
{
    public class DeleteUserCommandHandler : CommandHandler<DeleteUserCommand, bool>
    {
        private readonly ServerManDbContext _dbContext;

        public DeleteUserCommandHandler(ServerManDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Include(u => u.Organizations)
                .Include(u => u.Permissions)
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
                return false;

            // İlişkili verileri temizle
            _dbContext.UserOrganizations.RemoveRange(user.Organizations);
            _dbContext.UserPermissions.RemoveRange(user.Permissions);
            _dbContext.RefreshTokens.RemoveRange(user.RefreshTokens);

            // Kullanıcıyı sil
            _dbContext.Users.Remove(user);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}