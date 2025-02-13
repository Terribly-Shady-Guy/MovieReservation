using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using MovieReservation.Models;
using MovieReservation.ViewModels;
using System.Security.Claims;

namespace MovieReservation.Services
{
    public class LoginDto
    {
        public required SignInResult Result { get; set; }
        public AuthenticationToken? AuthToken { get; set; } = null;
    }

    public class AuthenticationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuthenticationTokenProvider _tokenProvider;

        public AuthenticationService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IAuthenticationTokenProvider tokenProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenProvider = tokenProvider;
        }

        public async Task<LoginDto> Login(UserLoginVM userCredentials)
        {
            AppUser? user = await _userManager.FindByNameAsync(userCredentials.Username);
            if (user == null)
            {
                return new LoginDto
                {
                    Result = SignInResult.Failed,
                };
            }

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, userCredentials.Password, true);
            
            if (!result.Succeeded)
            {
                return new LoginDto
                {
                    Result = result
                };
            }

            string twoFactorCode = await CheckAndGenerateTwoFactorCode(user);
            if (twoFactorCode != string.Empty)
            {
                return new LoginDto
                {
                    Result = SignInResult.TwoFactorRequired
                };
            }

            ClaimsIdentity accessTokenIdentity = await CreateClaimsIdentity(user);
            AuthenticationToken token = await _tokenProvider.GenerateTokens(accessTokenIdentity);

            user.RefreshToken = token.RefreshToken;
            user.ExpirationDate = token.RefreshExpiration;

            await _userManager.UpdateAsync(user);

            return new LoginDto
            {
                Result = SignInResult.Success,
                AuthToken = token
            };
        }

        public async Task<AuthenticationToken?> RefreshTokens(string access, string refresh)
        {
            var result = await _tokenProvider.ValidateExpiredToken(access);
            if (!result.IsValid || result.SecurityToken is not JsonWebToken accessToken)
            {
                return null;
            }

             bool claimResult = accessToken.TryGetClaim(JwtRegisteredClaimNames.Sub, out Claim userIdClaim);

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
            AuthenticationToken newToken = await _tokenProvider.GenerateTokens(identity);

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

        private async Task<ClaimsIdentity> CreateClaimsIdentity(AppUser user)
        {
            IList<string> userRoles = await _userManager.GetRolesAsync(user);

            var accessTokenIdentity = new ClaimsIdentity();
            accessTokenIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, user.Id));

            foreach (string role in userRoles)
            {
                accessTokenIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            return accessTokenIdentity;
        }

        private async Task<string> CheckAndGenerateTwoFactorCode(AppUser user)
        {
            bool is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            if (!is2faEnabled)
            {
                return string.Empty;
            }

            var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
            if (!providers.Any(p => p == "Email"))
            {
                return string.Empty;
            }
            
            return await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
        }
    }
}
