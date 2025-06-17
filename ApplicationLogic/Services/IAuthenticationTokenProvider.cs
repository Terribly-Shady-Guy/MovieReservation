using ApplicationLogic.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ApplicationLogic.Services
{
    public interface IAuthenticationTokenProvider
    {
        Task<AuthenticationToken> GenerateTokens(ClaimsIdentity identity);
        Task<TokenValidationResult> ValidateExpiredToken(string expiredToken);
    }
}