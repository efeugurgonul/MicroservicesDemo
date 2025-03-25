using Common.Core.Auth;
using Common.Core.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using ServerManagementService.Services;
using ServerManagementService.Data;
using System.Security.Authentication;

namespace ServerManagementService.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : CommandHandler<LoginCommand, AuthResponse>
    {
        private readonly ServerManDbContext _dbContext;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(
            ServerManDbContext dbContext,
            IPasswordService passwordService,
            ITokenService tokenService)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        public override async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

            if (user == null)
                throw new AuthenticationException("Kullanıcı adı veya şifre hatalı.");

            if (!_passwordService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                throw new AuthenticationException("Kullanıcı adı veya şifre hatalı.");

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

            // Eski refresh tokenları temizle
            var oldTokens = await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == user.Id)
                .ToListAsync(cancellationToken);

            _dbContext.RefreshTokens.RemoveRange(oldTokens);
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
