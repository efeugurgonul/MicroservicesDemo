using Common.Core.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using ServerManagementService.Data;
using ServerManagementService.Services;

namespace ServerManagementService.Features.Users.Commands.Update
{
    public class UpdateUserCommandHandler : CommandHandler<UpdateUserCommand, bool>
    {
        private readonly ServerManDbContext _dbContext;
        private readonly IPasswordService _passwordService;
        private readonly IOrganizationService _organizationService;

        public UpdateUserCommandHandler(
            ServerManDbContext dbContext,
            IPasswordService passwordService,
            IOrganizationService organizationService)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
            _organizationService = organizationService;
        }

        public override async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
                return false;

            // Kullanıcı temel bilgilerini güncelle
            if (!string.IsNullOrEmpty(request.Username))
                user.Username = request.Username;

            if (!string.IsNullOrEmpty(request.Email))
                user.Email = request.Email;

            // Şifre güncellemesi
            if (!string.IsNullOrEmpty(request.Password))
            {
                _passwordService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            // Aktiflik durumu
            if (request.IsActive.HasValue)
                user.IsActive = request.IsActive.Value;

            // Default organizasyon güncelleme
            if (request.DefaultOrganizationId.HasValue &&
                request.DefaultOrganizationId.Value != user.DefaultOrganizationId)
            {
                // Organizasyon erişimi var mı kontrol et ve ekle
                bool hasAccess = await _organizationService.UserHasOrganizationAccessAsync(
                    user.Id, request.DefaultOrganizationId.Value);

                if (!hasAccess)
                {
                    await _organizationService.AssignOrganizationToUserAsync(
                        user.Id, request.DefaultOrganizationId.Value);
                }

                user.DefaultOrganizationId = request.DefaultOrganizationId.Value;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}