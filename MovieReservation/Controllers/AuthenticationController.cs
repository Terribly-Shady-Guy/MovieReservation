using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MovieReservation.Models;
using MovieReservation.Services;
using MovieReservation.ViewModels;
using System.Security.Claims;

namespace MovieReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AuthenticationTokenManager _manager;

        public AuthenticationController(UserService userService, AuthenticationTokenManager manager)
        {
            _userService = userService;
            _manager = manager;
        }

        [HttpPost]
        public async Task<ActionResult> Login(UserLoginVM userLogin)
        {
            AppUser? user = await _userService.GetUserAsync(userLogin);

            if (user == null)
            {
                return Unauthorized(new {Message = $"User {userLogin.Username} does not exist."});
            }

            Token token = _manager.GenerateTokens(user);

            await _userService.UpdateRefreshToken(token.RefreshToken, token.RefreshExpiration, user);

            SetResfreshTokenCookie(token.RefreshToken, token.RefreshExpiration);

            return Ok(new { Token = token.AccessToken });
        }

        [HttpPut]
        public async Task<ActionResult> RefreshTokens(string expiredToken)
        {
            string? refreshToken = Request.Cookies["refresh-token"];

            if (refreshToken is null)
            {
                return Unauthorized(new { Message = "A refresh token is not available" });
            }

            TokenValidationResult result = await _manager.ValidateExpiredJwtToken(expiredToken);

            if (!result.IsValid)
            {
                return Unauthorized(new { Message = "This is not a valid refresh token" });
            }

            int id = GetUserIdFromClaims(result.ClaimsIdentity);

            AppUser? user = await _userService.GetUserAsync(id);

            if (user is null)
            {
                return NotFound(new { Message = "The user does not exist." });
            }

            if (user.RefreshToken != refreshToken || user.ExpirationDate <= DateTime.UtcNow)
            {
                return Unauthorized(new { Message = "the refresh token is not valid." });
            } 
            
            Token token = _manager.GenerateTokens(user);
            SetResfreshTokenCookie(token.RefreshToken, token.RefreshExpiration);

            return Ok(new { Token = token.AccessToken });
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            string? accessToken = await HttpContext.GetTokenAsync("access_token");

            if (accessToken is null)
            {
                return NotFound();
            }

            string? refreshToken = Request.Cookies["refresh-token"];

            if (refreshToken is null)
            {
                return Unauthorized();
            }

            TokenValidationResult result = await _manager.ValidateExpiredJwtToken(accessToken);

            if (!result.IsValid)
            {
                return Unauthorized();
            }

            int id = GetUserIdFromClaims(result.ClaimsIdentity);

            await _userService.UpdateRefreshToken(null, null, id);
            Response.Cookies.Delete("refresh-token");

            return NoContent();
        }

        private void SetResfreshTokenCookie(string token, DateTime expiration)
        {
            var options = new CookieOptions()
            {
                Expires = expiration,
                Secure = true,
                IsEssential = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
            };

            Response.Cookies.Append("refresh-token", token, options);
        }

        private int GetUserIdFromClaims(ClaimsIdentity identity)
        {
            int id = 0;
            foreach (var claim in identity.Claims)
            {
                if (claim.Type == ClaimTypes.NameIdentifier)
                {
                    id = int.Parse(claim.Value);
                }
            }

            return id;
        }
    }
}
