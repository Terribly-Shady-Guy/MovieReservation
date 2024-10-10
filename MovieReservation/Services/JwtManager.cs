

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MovieReservation.Models;
using System.Security.Claims;

namespace MovieReservation.Services
{
    public class JwtManager
    {
        private readonly IConfiguration _configuration;
        private readonly RsaSecurityKey _securityKey;

        public JwtManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwt(AppUser user)
        {
            var signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.RsaSha256Signature);

            Claim[] claims =
            [
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Username)
            ];

            var descriptor = new SecurityTokenDescriptor()
            {
                SigningCredentials = signingCredentials,
                Expires = DateTime.UtcNow.AddMinutes(10),
                Subject = new ClaimsIdentity(claims)
            };

            return new JsonWebTokenHandler().CreateToken(descriptor);
        }
    }
}
