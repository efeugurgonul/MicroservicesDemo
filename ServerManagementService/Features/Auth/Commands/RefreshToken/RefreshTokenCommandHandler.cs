using Common.Core.Auth;
using Common.Core.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using ServerManagementService.Controllers;
using ServerManagementService.Data;
using ServerManagementService.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ServerManagementService.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : CommandHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly ServerManDbContext _dbContext;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;

        public RefreshTokenCommandHandler(
            ServerManDbContext dbContext,
            ITokenService tokenService, 
            IPermissionService permissionService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
            _permissionService = permissionService;
        }

        public override async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // 1. Refresh token'ı bul ve geçerliliğini kontrol et
            var refreshToken = await _dbContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

            if (refreshToken == null)
                throw new UnauthorizedAccessException("Geçersiz refresh token.");

            if (!refreshToken.IsActive)
                throw new UnauthorizedAccessException("Refresh token aktif değil.");

            var user = refreshToken.User;

            // 2. Eski refresh token'ı geçersiz kıl
            refreshToken.Revoked = DateTime.UtcNow;

            // 3. Kullanıcının izinlerini ve organizasyonlarını al
            var permissions = await _permissionService.GetUserPermissionsAsync(user.Id);

            var organizations = await _dbContext.UserOrganizations
                .Where(uo => uo.UserId == user.Id)
                .Select(uo => uo.OrganizationId)
                .ToListAsync(cancellationToken);

            // 4. Claim'leri oluştur
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim("defaultOrganizationId", user.DefaultOrganizationId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // İzinleri claim'lere ekle
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission.ToString()));
            }

            // Organizasyonları claim'lere ekle
            foreach (var orgId in organizations)
            {
                claims.Add(new Claim("orgAccess", orgId.ToString()));
            }

            // 5. Yeni JWT token oluştur
            //var token = await _tokenService.GenerateTokenAsync(user.Id, user.Username, user.DefaultOrganizationId);
            var token = await _tokenService.GenerateTokenWithClaimsAsync(claims);

            // 6. Yeni refresh token oluştur
            var newRefreshToken = new Domain.Entities.RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };

            _dbContext.RefreshTokens.Add(newRefreshToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // 7. AuthResponse döndür
            return new AuthResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Token = token,
                RefreshToken = newRefreshToken.Token,
                DefaultOrganizationId = user.DefaultOrganizationId
            };
        }

    }
}
