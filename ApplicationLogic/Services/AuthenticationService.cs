using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using DbInfrastructure.Models;
using System.Security.Claims;
using ApplicationLogic.ViewModels;
using ApplicationLogic.Interfaces;

namespace ApplicationLogic.Services
{
    public class LoginDto
    {
        public required SignInResult Result { get; set; }
        public AuthenticationToken? AuthToken { get; set; } = null;
        public string? UserId { get; set; } = null;
    }

    public class AuthenticationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuthenticationTokenProvider _tokenProvider;
        private readonly LoginManager _login;

        public AuthenticationService(
            UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, 
            IAuthenticationTokenProvider tokenProvider, 
            LoginManager login)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenProvider = tokenProvider;
            _login = login;
        }

        public async Task<LoginDto> Login(UserLoginDto userCredentials)
        {
            AppUser? user = await _userManager.FindByEmailAsync(userCredentials.Email);

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

            bool is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            if (is2faEnabled)
            {
                string twoFactorCode = await GenerateTwoFactorCode(user);
                Console.WriteLine(twoFactorCode);

                return new LoginDto
                {
                    Result = SignInResult.TwoFactorRequired,
                    UserId = user.Id
                };
            }

            ClaimsIdentity accessTokenIdentity = await CreateClaimsIdentity(user);
            AuthenticationToken token = await _tokenProvider.GenerateTokens(accessTokenIdentity);

            await _login.CreateLogin(user, token);

            return new LoginDto
            {
                Result = SignInResult.Success,
                AuthToken = token
            };
        }

        public async Task<LoginDto> LoginWithTwoFactorCode(string twoFactorCode, string userId)
        {
            AppUser? user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return new LoginDto
                {
                    Result = SignInResult.Failed
                };
            }

            bool isLockedOut = await _userManager.IsLockedOutAsync(user);
            if (isLockedOut)
            {
                return new LoginDto { Result = SignInResult.LockedOut };
            }

            bool result = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", twoFactorCode);
            if (!result)
            {
                await _userManager.AccessFailedAsync(user);
                return new LoginDto
                {
                    Result = SignInResult.Failed
                };
            }
            
            ClaimsIdentity identity = await CreateClaimsIdentity(user);
            AuthenticationToken token = await _tokenProvider.GenerateTokens(identity);

            await _login.CreateLogin(user, token);

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

            InternalLogin? login = await _login.GetLoginSession(user, refresh);

            DateTime current = DateTime.UtcNow;
            if (login is null || current >= login.ExpirationDate)
            {
                return null;
            }

            var identity = new ClaimsIdentity(accessToken.Claims);
            AuthenticationToken newToken = await _tokenProvider.GenerateTokens(identity);

            await _login.UpdateTokens(login, newToken);

            return newToken;
        }

        public async Task Logout(string userId, string refreshToken)
        {
            AppUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null) { return; }

            InternalLogin? login = await _login.GetLoginSession(user, refreshToken);

            if (login is null)
            {
                return;
            }

            await _login.DeleteLogin(login);
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

        private async Task<string> GenerateTwoFactorCode(AppUser user)
        {
            const string EmailProviderName = "Email";

            var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
            if (!providers.Any(p => p == EmailProviderName))
            {
                return string.Empty;
            }
            
            return await _userManager.GenerateTwoFactorTokenAsync(user, EmailProviderName);
        }
    }
}
