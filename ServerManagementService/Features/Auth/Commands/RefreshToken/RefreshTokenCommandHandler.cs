using Common.Core.Auth;
using Common.Core.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using ServerManagementService.Data;

namespace ServerManagementService.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : CommandHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly ServerManDbContext _dbContext;
        private readonly ITokenService _tokenService;

        public RefreshTokenCommandHandler(
            ServerManDbContext dbContext,
            ITokenService tokenService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
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

            // 3. Yeni JWT token oluştur
            var token = await _tokenService.GenerateTokenAsync(user.Id, user.Username, user.DefaultOrganizationId);

            // 4. Yeni refresh token oluştur
            var newRefreshToken = new Domain.Entities.RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };

            _dbContext.RefreshTokens.Add(newRefreshToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // 5. AuthResponse döndür
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
