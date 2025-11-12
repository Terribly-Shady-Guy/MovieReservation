using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;
using MovieReservation.OpenApi;
using MovieReservation.OpenApi.Transformers;
using ApplicationLogic.Services;
using ApplicationLogic.ViewModels;
using Asp.Versioning;
using System.Net.Mime;
using Microsoft.AspNetCore.RateLimiting;

namespace MovieReservation.Controllers
{
    [ApiVersion(1.0)]
    [EnableRateLimiting("Authentication")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService _authentication;

        public AuthenticationController(AuthenticationService authentication)
        {
            _authentication = authentication;
        }

        [MapToApiVersion(1.0)]
        [EndpointSummary("Login")]
        [ProducesResponseType<AuthenticationToken>(StatusCodes.Status200OK, Description = "Authentication was successful.")]
        [ProducesResponseType(StatusCodes.Status202Accepted, Description = "Authentication was successful, but 2fa is required.")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, Description = "The user does not exist or password is incorrect.")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden, Description = "The user account is locked.")]
        [Produces(MediaTypeNames.Application.Json)]
        [OperationTransformer<LoginEndpointTransformer>]
        [EndpointDescription("An endpoint for user login. This endpoint handles authentication using a jwt access token and a randomly generated refresh token.")]
        [HttpPost("Login")]
        public async Task<ActionResult<AuthenticationToken>> Login([Description("Object containing username and password for login")] UserLoginDto userLogin)
        {
            LoginDto login = await _authentication.Login(userLogin);
            
            if (login.Result is { Succeeded: false, RequiresTwoFactor: false })
            {
                return Problem(
                    title: "Login failed",
                    detail: "The provided username or password is incorrect.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }
            else if (login.Result.RequiresTwoFactor && login.UserId is not null)
            {
                return Accepted(new TwoFactorMessage 
                { 
                    Message = "Two factor authentication is required. Please check your email for code.",
                    UserId = login.UserId
                });
            }
            else if (login.Result.IsLockedOut)
            {
                return Problem(
                    title: "Account locked",
                    detail: "The current account could not be logged in as it is currently locked.",
                    statusCode: StatusCodes.Status403Forbidden);
            }

            return Ok(login.AuthToken);
        }

        [MapToApiVersion(1.0)]
        [EndpointSummary("Two factor login")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, Description = "The provided code is invalid or user doesn't exist.")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden, Description = "The user account is locked.")]
        [ProducesResponseType<AuthenticationToken>(StatusCodes.Status200OK, Description = "Two factor authentication is successful.")]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpPost("Two-Factor-Login")]
        public async Task<ActionResult<AuthenticationToken>> LoginWithTwoFactor(string twoFactorCode, string userId)
        {
            var login = await _authentication.LoginWithTwoFactorCode(twoFactorCode, userId);

            if (!login.Result.Succeeded && !login.Result.IsLockedOut)
            {
                return Problem(
                    title: "2fa failed",
                    detail: "The provided id or code is invalid.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }
            else if (login.Result.IsLockedOut)
            {
                return Problem(
                    title: "Account locked",
                    detail: "The current account could not be logged in as it is currently locked.",
                    statusCode: StatusCodes.Status403Forbidden);
            }

            return Ok(login.AuthToken);
        }

        [MapToApiVersion(1.0)]
        [EndpointSummary("Refresh tokens")]
        [ProducesResponseType<AuthenticationToken>(StatusCodes.Status200OK, Description = "The token was successfully refreshed.")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Description = "Refresh or access token is invalid")]
        [Produces(MediaTypeNames.Application.Json)]
        [EndpointDescription("An endpoint for refreshing the access token. This endpoint will validate the access token and matches refresh token to database login.")]
        [HttpPatch]
        public async Task<ActionResult<AuthenticationToken>> RefreshTokens([Description("An object containing the expired access token and valid refresh token.")] AuthenticationTokenRequestBody expiredToken)
        {
            AuthenticationToken? token = await _authentication.RefreshTokens(expiredToken.AccessToken, expiredToken.RefreshToken);

            if (token == null)
            {
                return Problem(
                    title: "Invalid tokens",
                    detail: "The provided access or refresh token is invalid.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            return Ok(token);
        }

        [MapToApiVersion(1.0)]
        [EndpointSummary("Logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Description = "User successfully logged out.")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Description = "Access or refresh token is missing.")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, Description = "User does not exist.")]
        [EndpointDescription("An endpoint for handling logout. This endpoint will delete the login session from db when successful.")]
        [Authorize]
        [DisableRateLimiting]
        [HttpDelete]
        public async Task<ActionResult> Logout(string refreshToken)
        {
            try
            {
                Claim? userIdClaim = User.Claims
                    .SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim is null)
                {
                    return Problem(
                        title: "Invalid token",
                        detail: "The provided access token is invalid.",
                        statusCode: StatusCodes.Status400BadRequest);
                }

                await _authentication.Logout(userIdClaim.Value, refreshToken);

                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return Problem(
                    title: "Invalid token",
                    detail: "The provided access token is invalid.",
                    statusCode: StatusCodes.Status400BadRequest);
            }   
        }
    }
}
