using Microsoft.IdentityModel.Tokens;
using DbInfrastructure.ViewModels;
using System.Security.Claims;

namespace DbInfrastructure.Services
{
    public interface IAuthenticationTokenProvider
    {
        Task<AuthenticationToken> GenerateTokens(ClaimsIdentity identity);
        Task<TokenValidationResult> ValidateExpiredToken(string expiredToken);
    }
}