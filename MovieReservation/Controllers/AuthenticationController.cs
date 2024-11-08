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

        /// <summary>Endpoint for user login</summary>
        /// <param name="userLogin">Object containing username and password for login</param>
        /// <returns>An object containing the jwt access token</returns>
        /// <remarks>
        /// This endpoint also sets a cookie containing a refresh token.
        /// </remarks>
        /// <response code="200">Returns the token in a json object</response>
        /// <response code="401">If the user does not exist or password is incorrect</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
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
        /// Endpoint for refreshing access token.
        /// </summary>
        /// <param name="expiredToken">The expired access token</param>
        /// <returns>A new access token</returns>
        /// <response code="200">The token was successfully refreshed</response>
        /// <response code="401">Refresh or access token is invalid</response>
        /// <response code="404">The user associated with the token does not exist</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpPut]
        public async Task<ActionResult<AccessTokenResponse>> RefreshTokens(string expiredToken)
        {
            string? refreshToken = Request.Cookies["refresh-token"];

            if (refreshToken is null)
            {
                return Unauthorized(new { Message = "A refresh token is not available" });
            }

            TokenValidationResult result = await _manager.ValidateExpiredJwtToken(expiredToken);

            if (!result.IsValid)
            {
                return Unauthorized(new { Message = "This is not a valid access token" });
            }
            
            Claim? idClaim = result.ClaimsIdentity.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (idClaim is null)
            {
                return Unauthorized();
            }

            AppUser? user = await _userService.GetUserAsync(int.Parse(idClaim.Value));

            if (user is null)
            {
                return NotFound(new { Message = "The user does not exist." });
            }

            if (user.RefreshToken != refreshToken || user.ExpirationDate <= DateTime.UtcNow)
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
        /// Endpoint for handling logout.
        /// </summary>
        /// <returns></returns>
        /// <response code="204">User successfully logged out</response>
        /// <response code="401">access or refresh token is missing</response>
        /// <response code="404">user does not exist</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            Claim? idClaim = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (idClaim is null)
            {
                return NotFound();
            }

            int id = int.Parse(idClaim.Value);

            string? refreshToken = Request.Cookies["refresh-token"];

            if (refreshToken is null)
            {
                return Unauthorized();
            }

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
    }
}
