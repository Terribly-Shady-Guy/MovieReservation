using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MovieReservation.Models;
using MovieReservation.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MovieReservation.Services
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
    public class AuthenticationTokenProvider
    {
        private readonly IOptionsMonitor<JwtOptions> _options;
        private readonly IRsaKeyHandler _securityKeyHandler;

        public AuthenticationTokenProvider(IOptionsMonitor<JwtOptions> options, IRsaKeyHandler securityKeyHandler)
        {
            _options = options;
            _securityKeyHandler = securityKeyHandler;
        }

        public async Task<Token> GenerateTokens(AppUser user, ClaimsIdentity identity)
        {
            var tokenModel = new Token
            {
                AccessToken = await GenerateAccessToken(identity)
            };

            tokenModel = GenerateRefreshToken(tokenModel);

            return tokenModel;
        }

        public async Task<TokenValidationResult> ValidateExpiredJwtToken(string expiredToken)
        {
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
                ValidAudience = _options.CurrentValue.Audience,
                ValidIssuer = _options.CurrentValue.Issuer,
            };

            var handler = new JsonWebTokenHandler();
            var result = await handler.ValidateTokenAsync(expiredToken, tokenParams);

            return result;
        }

        private async Task<string> GenerateAccessToken(ClaimsIdentity identity)
        {
            RsaSecurityKey securityKey = await _securityKeyHandler.LoadPrivateAsync();

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

            var descriptor = new SecurityTokenDescriptor()
            {
                SigningCredentials = signingCredentials,
                Expires = DateTime.UtcNow.AddMinutes(10),
                Subject = identity,
                Issuer = _options.CurrentValue.Issuer,
                Audience = _options.CurrentValue.Audience,
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
