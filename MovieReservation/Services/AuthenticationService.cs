using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using DbInfrastructure.Models;
using MovieReservation.ViewModels;
using System.Security.Claims;
using DbInfrastructure;

namespace MovieReservation.Services
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
        private readonly MovieReservationDbContext _dbContext;

        public AuthenticationService(
            UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, 
            IAuthenticationTokenProvider tokenProvider, 
            MovieReservationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenProvider = tokenProvider;
            _dbContext = context;
        }

        public async Task<LoginDto> Login(UserLoginVM userCredentials)
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

            await CreateLogin(user, token);

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

            await CreateLogin(user, token);

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

            InternalLogin? login = await GetLoginSession(user, refresh);

            DateTime current = DateTime.UtcNow;
            if (login is null || current >= login.ExpirationDate)
            {
                return null;
            }

            var identity = new ClaimsIdentity(accessToken.Claims);
            AuthenticationToken newToken = await _tokenProvider.GenerateTokens(identity);

            login.RefreshToken = newToken.RefreshToken;
            login.ExpirationDate = newToken.RefreshExpiration;

            _dbContext.Logins
                .Entry(login)
                .Property(l => l.RefreshToken)
                .IsModified = true;

            _dbContext.Logins
                .Entry(login)
                .Property(l => l.ExpirationDate)
                .IsModified = true;
            
            await _dbContext.SaveChangesAsync();

            return newToken;
        }

        public async Task Logout(string userId, string refreshToken)
        {
            AppUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null) { return; }

            InternalLogin? login = await GetLoginSession(user, refreshToken);

            if (login is null)
            {
                return;
            }

            _dbContext.Logins.Remove(login);
            await _dbContext.SaveChangesAsync();
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

        private async Task CreateLogin(AppUser user, AuthenticationToken token)
        {
            var newLogin = new InternalLogin
            {
                LoginId = Guid.NewGuid().ToString(),
                RefreshToken = token.RefreshToken,
                ExpirationDate = token.RefreshExpiration,
                UserId = user.Id,
                LoggedInUser = user
            };

            _dbContext.Logins.Add(newLogin);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<InternalLogin?> GetLoginSession(AppUser user, string refreshToken)
        {
            return await _dbContext.Logins
                .Where(l => l.UserId == user.Id && l.RefreshToken == refreshToken)
                .FirstOrDefaultAsync();
        }
    }
}
