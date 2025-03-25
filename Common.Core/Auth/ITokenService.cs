using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.Auth
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(int userId, string username, int defaultOrganizationId);
        bool ValidateToken(string token, out JwtSecurityToken validatedToken);
        int GetUserIdFromToken(JwtSecurityToken token);
        int GetDefaultOrganizationIdFromToken(JwtSecurityToken token);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> GenerateTokenAsync(int userId, string username, int defaultOrganizationId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim("defaultOrganizationId", defaultOrganizationId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string token, out JwtSecurityToken validatedToken)
        {
            validatedToken = null;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]); ;

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken securityToken);

                validatedToken = securityToken as JwtSecurityToken;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int GetUserIdFromToken(JwtSecurityToken token)
        {
            var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }

        public int GetDefaultOrganizationIdFromToken(JwtSecurityToken token)
        {
            var orgIdClaim = token.Claims.FirstOrDefault(c => c.Type == "defaultOrganizationId");
            if (orgIdClaim != null && int.TryParse(orgIdClaim.Value, out int defaultOrganizationId))
            {
                return defaultOrganizationId;
            }
            return 0;
        }
    }
}
