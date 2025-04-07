using Common.Core.Auth;
using Common.Core.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using ServerManagementService.Domain.Entities;
using ServerManagementService.Services;
using ServerManagementService.Data;
using ServerManagementService.Controllers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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
                Permission.ViewProducts
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

            // İzinleri ve organizasyonları claim'lere ekle
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim("defaultOrganizationId", user.DefaultOrganizationId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Temel izinleri claim'lere ekle
            foreach (var permission in basicPermissions)
            {
                claims.Add(new Claim("permission", permission.ToString()));
            }

            // Kullanıcının erişebildiği organizasyonu claim'e ekle
            claims.Add(new Claim("orgAccess", request.OrganizationId.ToString()));

            // JWT token oluştur
            //var token = await _tokenService.GenerateTokenAsync(user.Id, user.Username, user.DefaultOrganizationId);
            var token = await _tokenService.GenerateTokenWithClaimsAsync(claims);

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
