using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MovieReservation.Models;
using MovieReservation.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MovieReservation.Services
{
    public class AuthenticationTokenManager
    {
        private readonly IConfiguration _configuration;
        private readonly IRsaKeyHandler _securityKeyHandler;

        public AuthenticationTokenManager(IConfiguration configuration, IRsaKeyHandler securityKeyHandler)
        {
            _configuration = configuration;
            _securityKeyHandler = securityKeyHandler;
        }

        public async Task<Token> GenerateTokens(AppUser user)
        {
            Claim[] claims =
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                //new Claim(ClaimTypes.Role, user.Role),
            ];

            var tokenModel = new Token
            {
                AccessToken = await GenerateAccessToken(claims)
            };

            tokenModel = GenerateRefreshToken(tokenModel);

            return tokenModel;
        }

        public async Task<TokenValidationResult> ValidateExpiredJwtToken(string expiredToken)
        {
            var jwtConfig = _configuration.GetSection("Jwt");
            RsaSecurityKey securityKey = await _securityKeyHandler.LoadPublicAsync();

            var tokenParams = new TokenValidationParameters
            {
                ValidateLifetime = false,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidAlgorithms = [
                    SecurityAlgorithms.RsaSha256
                ],
                IssuerSigningKey = securityKey,
                ValidAudience = jwtConfig.GetValue<string>("Audience"),
                ValidIssuer = jwtConfig.GetValue<string>("Issuer")
            };

            var handler = new JsonWebTokenHandler();

            var result = await handler.ValidateTokenAsync(expiredToken, tokenParams);

            return result;
        }

        private async Task<string> GenerateAccessToken(Claim[] claims)
        {
            RsaSecurityKey securityKey = await _securityKeyHandler.LoadPrivateAsync();

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
            var jwtConfig = _configuration.GetSection("Jwt");

            var descriptor = new SecurityTokenDescriptor()
            {
                SigningCredentials = signingCredentials,
                Expires = DateTime.UtcNow.AddMinutes(10),
                Subject = new ClaimsIdentity(claims),
                Issuer = jwtConfig.GetValue<string>("Issuer"),
                Audience = jwtConfig.GetValue<string>("Audience")
            };

            return new JsonWebTokenHandler().CreateToken(descriptor);

        }

        private Token GenerateRefreshToken(Token tokenModel)
        {
            tokenModel.RefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            tokenModel.RefreshExpiration = DateTime.UtcNow.AddDays(4);

            return tokenModel;
        }
    }
}
