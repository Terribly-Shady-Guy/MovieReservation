using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using MovieReservation.Models;
using MovieReservation.ViewModels;
using System.Security.Claims;

namespace MovieReservation.Services
{
    public class AuthenticationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AuthenticationTokenProvider _tokenProvider;

        public AuthenticationService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AuthenticationTokenProvider tokenProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenProvider = tokenProvider;
        }

        public async Task<Token?> Login(UserLoginVM userCredentials)
        {
            AppUser? user = await _userManager.FindByNameAsync(userCredentials.Username);
            if (user == null) { return null; }

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, userCredentials.Password, true);

            if (!result.Succeeded)
            {
                return null;
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            List<Claim> claims = [
                new Claim(ClaimTypes.NameIdentifier, user.Id)
                ];

            foreach (string role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            Token token = await _tokenProvider.GenerateTokens(user, new ClaimsIdentity(claims));

            user.RefreshToken = token.RefreshToken;
            user.ExpirationDate = token.RefreshExpiration;

            await _userManager.UpdateAsync(user);

            return token;
        }

        public async Task<Token?> RefreshTokens(string access, string refresh)
        {
            var result = await _tokenProvider.ValidateExpiredJwtToken(access);
            if (!result.IsValid || result.SecurityToken is not JsonWebToken accessToken)
            {
                return null;
            }

             bool claimResult = accessToken.TryGetClaim(ClaimTypes.NameIdentifier, out Claim userIdClaim);

            if (!claimResult)
            {
                return null;
            }

            AppUser? user = await _userManager.FindByIdAsync(userIdClaim.Value);
            if (user == null) 
            {
                return null; 
            }

            DateTime current = DateTime.UtcNow;
            if (refresh != user.RefreshToken || current >= user.ExpirationDate)
            {
                return null;
            }
            
            var identity = new ClaimsIdentity(accessToken.Claims);
            Token newToken = await _tokenProvider.GenerateTokens(user, identity);

            user.RefreshToken = newToken.RefreshToken;
            user.ExpirationDate = newToken.RefreshExpiration;
            await _userManager.UpdateAsync(user);

            return newToken;
        }

        public async Task Logout(string userId)
        {
            AppUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null) { return; }

            user.ExpirationDate = null;
            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
        }
    }
}
