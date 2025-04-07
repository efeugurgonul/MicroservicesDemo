using Common.Core.Auth;
using Common.Core.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using ServerManagementService.Services;
using ServerManagementService.Data;
using System.Security.Authentication;
using ServerManagementService.Controllers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ServerManagementService.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : CommandHandler<LoginCommand, AuthResponse>
    {
        private readonly ServerManDbContext _dbContext;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;

        public LoginCommandHandler(
            ServerManDbContext dbContext,
            IPasswordService passwordService,
            ITokenService tokenService,
            IPermissionService permissionService)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
            _tokenService = tokenService;
            _permissionService = permissionService;
        }

        public override async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

            if (user == null)
                throw new AuthenticationException("Kullanıcı adı veya şifre hatalı.");

            if (!_passwordService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                throw new AuthenticationException("Kullanıcı adı veya şifre hatalı.");

            // İzinleri çek
            var permissions = await _permissionService.GetUserPermissionsAsync(user.Id);

            // Kullanıcıya ait organizasyonları çek (opsiyonel)
            var organizations = await _dbContext.UserOrganizations
                .Where(uo => uo.UserId == user.Id)
                .Select(uo => uo.OrganizationId)
                .ToListAsync(cancellationToken);

            // Token için claim'ler oluştur
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

            // Organizasyonları claim'lere ekle (opsiyonel)
            foreach (var orgId in organizations)
            {
                claims.Add(new Claim("orgAccess", orgId.ToString()));
            }


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
