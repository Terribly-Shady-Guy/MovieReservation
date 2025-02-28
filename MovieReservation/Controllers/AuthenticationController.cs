using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.Services;
using MovieReservation.ViewModels;
using System.ComponentModel;
using System.Security.Claims;

namespace MovieReservation.Controllers
{
    [Route("api/[controller]/[action]")]
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
        /// This endpoint handles authentication using a jwt as an access token and uses a basic refresh token.
        /// The refresh token and its expiration date and time will be stored in the database.
        /// </remarks>
        /// <response code="200">Authentication was successful</response>
        /// <response code="202">Authentication was successful, but 2fa is required</response>
        /// <response code="401">The user does not exist or password is incorrect</response>
        [ProducesResponseType<AuthenticationToken>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status102Processing)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Produces("application/json")]
        [EndpointDescription("An endpoint for user login. This endpoint handles authentication using a jwt access token and a randomly generated refresh token.")]
        [HttpPost]
        public async Task<ActionResult> Login([Description("Object containing username and password for login")]UserLoginVM userLogin)
        {
            LoginDto login = await _authentication.Login(userLogin);

            if (!login.Result.Succeeded && !login.Result.RequiresTwoFactor)
            {
                return Unauthorized();
            }
            else if (login.Result.RequiresTwoFactor)
            {
                return Accepted(new { 
                    Message = "Two factor authentication is required. Please check your email for code",
                    login.UserId
                });
            }

            return Ok(login.AuthToken);
        }

        [HttpPost]
        public async Task<ActionResult> LoginWithTwoFactor(string twoFactorCode, string userId)
        {
            return Ok();
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
        [ProducesResponseType<AuthenticationToken>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [EndpointDescription("An endpoint for refreshing the access token. This endpoint will validate the access token and matches refresh token to database login.")]
        [HttpPatch]
        public async Task<ActionResult<AuthenticationToken>> RefreshTokens([Description("An object containing the expired access token and valid refresh token.")]AuthenticationTokenRequestBody expiredToken)
        {
            AuthenticationToken? token = await _authentication.RefreshTokens(expiredToken.AccessToken, expiredToken.RefreshToken);

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(token);
        }

        /// <summary>
        /// An endpoint for handling logout.
        /// </summary>
        /// <returns></returns>
        /// <remarks>This endpoint deletes the refresh token stored in the db upon successful authorization.</remarks>
        /// <response code="204">User successfully logged out</response>
        /// <response code="401">Access or refresh token is missing</response>
        /// <response code="404">User does not exist</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EndpointDescription("An endpoint for handling logout. This endpoint will delete the login session from db when sucessful.")]
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Logout(string refreshToken)
        {
            Claim? userIdClaim = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim is null)
            {
                return NotFound();
            }

           await _authentication.Logout(userIdClaim.Value, refreshToken);

            return NoContent();
        }
    }
}
