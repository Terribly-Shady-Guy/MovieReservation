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

        /// <summary>An endpoint for user login</summary>
        /// <param name="userLogin">Object containing username and password for login</param>
        /// <returns>An object containing the jwt access token</returns>
        /// <remarks>
        /// This endpoint handles authentication using a jwt as an access token and sets a cookie containing the refresh token.
        /// The refresh token and its expiration date and time will be stored in the database.
        /// </remarks>
        /// <response code="200">Authentication was successful</response>
        /// <response code="401">The user does not exist or password is incorrect</response>
        [ProducesResponseType<AccessTokenResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Produces("application/json")]
        [HttpPost]
        public async Task<ActionResult<AccessTokenResponse>> Login(UserLoginVM userLogin)
        {
            AppUser? user = await _userService.GetUserAsync(userLogin);

            if (user == null)
            {
                return Unauthorized($"User {userLogin.Username} does not exist.");
            }

            Token token = await _manager.GenerateTokens(user);

            await _userService.UpdateRefreshToken(token.RefreshToken, token.RefreshExpiration, user);

            SetResfreshTokenCookie(token.RefreshToken, token.RefreshExpiration);

            return Ok(new AccessTokenResponse
            {
                Token = token.AccessToken 
            });
        }

        /// <summary>
        /// An endpoint for refreshing access token.
        /// </summary>
        /// <param name="expiredToken">The expired access token</param>
        /// <returns>A new access token</returns>
        /// <remarks>This endpoint validates the access token and creates a new access and refresh token when valid.</remarks>
        /// <response code="200">The token was successfully refreshed</response>
        /// <response code="401">Refresh or access token is invalid</response>
        /// <response code="404">The user associated with the token does not exist</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpPatch]
        public async Task<ActionResult<AccessTokenResponse>> RefreshTokens(AccessTokenResponse expiredToken)
        {
            string? refreshToken = Request.Cookies["refresh-token"];

            if (refreshToken is null)
            {
                return Unauthorized(new { Message = "A refresh token is not available" });
            }

            TokenValidationResult result = await _manager.ValidateExpiredJwtToken(expiredToken.Token);

            if (!result.IsValid)
            {
                return Unauthorized(new { Message = "This is not a valid access token" });
            }
            
            Claim? userIdClaim = result.ClaimsIdentity.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            int userId;
            if (userIdClaim is null)
            {
                return Unauthorized();
            }
            else if (!int.TryParse(userIdClaim.Value, out userId))
            {
                return BadRequest();
            }

            AppUser? user = await _userService.GetUserAsync(userId);

            if (user is null)
            {
                return NotFound(new { Message = "The user does not exist." });
            }
            else if (user.RefreshToken != refreshToken || user.ExpirationDate <= DateTime.UtcNow)
            {
                return Unauthorized(new { Message = "the refresh token is not valid." });
            } 
            
            Token token = await _manager.GenerateTokens(user);

            await _userService.UpdateRefreshToken(token.RefreshToken, token.RefreshExpiration, user);
            SetResfreshTokenCookie(token.RefreshToken, token.RefreshExpiration);

            return Ok(new AccessTokenResponse
            {
                Token = token.AccessToken
            });
        }

        /// <summary>
        /// An endpoint for handling logout.
        /// </summary>
        /// <returns></returns>
        /// <remarks>This endpoint deletes the refresh token stored in the db and the cookie upon successful authorization.</remarks>
        /// <response code="204">User successfully logged out</response>
        /// <response code="401">Access or refresh token is missing</response>
        /// <response code="404">User does not exist</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            Claim? userIdClaim = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim is null)
            {
                return NotFound();
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest();
            }

            string? refreshToken = Request.Cookies["refresh-token"];

            if (refreshToken is null)
            {
                return Unauthorized();
            }

            await _userService.UpdateRefreshToken(null, null, userId);
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
    }
}
