using Microsoft.IdentityModel.Tokens;
using MovieReservation.ViewModels;
using System.Security.Claims;

namespace MovieReservation.Services
{
    public interface IAuthenticationTokenProvider
    {
        Task<AuthenticationToken> GenerateTokens(ClaimsIdentity identity);
        Task<TokenValidationResult> ValidateExpiredToken(string expiredToken);
    }
}