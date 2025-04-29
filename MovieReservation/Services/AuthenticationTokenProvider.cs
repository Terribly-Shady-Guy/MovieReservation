using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MovieReservation.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MovieReservation.Services
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int LifetimeMinutes { get; set; }
    }

    public class AuthenticationTokenProvider : IAuthenticationTokenProvider
    {
        private readonly IOptions<JwtOptions> _options;
        private readonly IRsaKeyHandler _securityKeyHandler;
        private readonly JsonWebTokenHandler _jwtHandler = new JsonWebTokenHandler();

        public AuthenticationTokenProvider(IOptions<JwtOptions> options, IRsaKeyHandler securityKeyHandler)
        {
            _options = options;
            _securityKeyHandler = securityKeyHandler;
        }

        public async Task<AuthenticationToken> GenerateTokens(ClaimsIdentity identity)
        {
            var tokenModel = new AuthenticationToken
            {
                AccessToken = await GenerateAccessToken(identity)
            };

            return GenerateRefreshToken(tokenModel);
        }

        public async Task<TokenValidationResult> ValidateExpiredToken(string expiredToken)
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
                ValidAudience = _options.Value.Audience,
                ValidIssuer = _options.Value.Issuer,
            };

            var result = await _jwtHandler.ValidateTokenAsync(expiredToken, tokenParams);

            return result;
        }

        private async Task<string> GenerateAccessToken(ClaimsIdentity identity)
        {
            RsaSecurityKey securityKey = await _securityKeyHandler.LoadPrivateAsync();
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
            
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                SigningCredentials = signingCredentials,
                Expires = DateTime.UtcNow.AddMinutes(_options.Value.LifetimeMinutes),
                Subject = identity,
                Issuer = _options.Value.Issuer,
                Audience = _options.Value.Audience,
                IssuedAt = DateTime.UtcNow,
            };

            return _jwtHandler.CreateToken(tokenDescriptor);
        }

        private static AuthenticationToken GenerateRefreshToken(AuthenticationToken tokenModel)
        {
            byte[] refreshTokenBytes = RandomNumberGenerator.GetBytes(200);
            tokenModel.RefreshToken = Convert.ToBase64String(refreshTokenBytes);
            tokenModel.RefreshExpiration = DateTime.UtcNow.AddDays(4);

            return tokenModel;
        }
    }
}
