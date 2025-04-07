using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiGateway.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await AttachUserToContextAsync(context, token);

            await _next(context);
        }

        private async Task AttachUserToContextAsync(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]);

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
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "sub").Value);
                var defaultOrgClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "defaultOrganizationId");
                var defaultOrganizationId = defaultOrgClaim != null ? int.Parse(defaultOrgClaim.Value) : 0;

                // Tüm claim'leri ClaimsIdentity'e aktarma
                var identity = new ClaimsIdentity(new List<Claim>(), "jwt");

                // Mevcut claim'leri ekle
                foreach (var claim in jwtToken.Claims)
                {
                    // Debug bilgisi
                    Console.WriteLine($"Adding claim to identity: {claim.Type}={claim.Value}");
                    identity.AddClaim(new Claim(claim.Type, claim.Value));
                }

                // Permission claim'lerinin özel bir kontrolü
                var permissionClaims = jwtToken.Claims.Where(c => c.Type == "permission").ToList();
                Console.WriteLine($"Found {permissionClaims.Count} permission claims in token");
                foreach (var permClaim in permissionClaims)
                {
                    Console.WriteLine($"Permission claim: {permClaim.Value}");
                }

                // ClaimsPrincipal oluştur ve HttpContext.User'a ata
                context.User = new ClaimsPrincipal(identity);

                // Kolay erişim için Items koleksiyonuna da ekle
                context.Items["UserId"] = userId;
                context.Items["DefaultOrganizationId"] = defaultOrganizationId;
                context.Items["JWT"] = jwtToken;

                Console.WriteLine($"Successfully attached user {userId} to context");
            }
            catch (Exception ex)
            {
                // Hata durumunda detaylı log
                Console.WriteLine($"JWT Middleware Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
    }
}
