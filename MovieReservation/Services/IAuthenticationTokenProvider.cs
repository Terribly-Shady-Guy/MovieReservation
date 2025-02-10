using Microsoft.IdentityModel.Tokens;
using MovieReservation.Models;
using MovieReservation.ViewModels;
using System.Security.Claims;

namespace MovieReservation.Services
{
    public interface IAuthenticationTokenProvider
    {
        Task<AuthenticationToken> GenerateTokens(AppUser user, ClaimsIdentity identity);
        Task<TokenValidationResult> ValidateExpiredToken(string expiredToken);
    }
}