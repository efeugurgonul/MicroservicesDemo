using Common.Core.Auth;
using Common.Core.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using ServerManagementService.Domain.Entities;
using ServerManagementService.Services;
using ServerManagementService.Data;

namespace ServerManagementService.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : CommandHandler<RegisterCommand, AuthResponse>
    {
        private readonly ServerManDbContext _dbContext;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public RegisterCommandHandler(
            ServerManDbContext dbContext,
            IPasswordService passwordService,
            ITokenService tokenService)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }
        
        public override async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Kullanıcı adı veya email kullanılıyor mu kontrol et
            if (await _dbContext.Users.AnyAsync(u => u.Username == request.Username || u.Email == request.Email, cancellationToken))
                throw new InvalidOperationException("Bu kullanıcı adı veya email zaten kullanılıyor.");

            // Organizasyon var mı kontrol et
            var organization = await _dbContext.Organizations.FindAsync(new object[] { request.OrganizationId }, cancellationToken);
            if (organization == null)
                throw new InvalidOperationException("Belirtilen organizasyon bulunamadı.");

            // Şifre hash'leme
            _passwordService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            // Yeni kullanıcı oluştur
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                DefaultOrganizationId = request.OrganizationId,
                IsActive = true
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Kullanıcıyı organizasyona ekle
            _dbContext.UserOrganizations.Add(new UserOrganization
            {
                UserId = user.Id,
                OrganizationId = request.OrganizationId
            });

            // Temel izinleri ekle (örnek)
            var basicPermissions = new[]
            {
                Permission.ViewOrganizations,
                Permission.ViewProducts,
                Permission.ViewLicenses,
                Permission.ViewParameters,
                Permission.ViewSchedules,
                Permission.ViewTerms
            };

            foreach (var permission in basicPermissions)
            {
                _dbContext.UserPermissions.Add(new UserPermission
                {
                    UserId = user.Id,
                    Permission = permission
                });
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            // JWT token oluştur
            var token = await _tokenService.GenerateTokenAsync(user.Id, user.Username, user.DefaultOrganizationId);

            // Refresh token oluştur
            var refreshToken = new Domain.Entities.RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };

            _dbContext.RefreshTokens.Add(refreshToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new AuthResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Token = token,
                RefreshToken = refreshToken.Token,
                DefaultOrganizationId = user.DefaultOrganizationId
            };
        }
    }
}
