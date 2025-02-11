using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.Services;
using MovieReservation.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MovieReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService _authentication;

        public AuthenticationController(AuthenticationService authentication)
        {
            _authentication = authentication;
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
            AuthenticationToken? token = await _authentication.Login(userLogin);

            if (token == null)
            {
                return Unauthorized();
            }

            SetResfreshTokenCookie(token.RefreshToken, token.RefreshExpiration);
            return Ok(new AccessTokenResponse { Token = token.AccessToken });
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
            string? refresh = Request.Cookies["refresh-token"];
            if (refresh == null)
            {
                return Unauthorized();
            }

            AuthenticationToken? token = await _authentication.RefreshTokens(expiredToken.Token, refresh);

            if (token == null)
            {
                return Unauthorized();
            }

            SetResfreshTokenCookie(token.RefreshToken, token.RefreshExpiration);
            return new AccessTokenResponse { Token = token.AccessToken };
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

            string? refreshToken = Request.Cookies["refresh-token"];

            if (refreshToken is null)
            {
                return Unauthorized();
            }

           await _authentication.Logout(userIdClaim.Value);
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
