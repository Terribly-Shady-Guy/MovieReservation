﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;
using MovieReservation.OpenApi;
using MovieReservation.OpenApi.Transformers;
using ApplicationLogic.Services;
using ApplicationLogic.ViewModels;

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

        [EndpointSummary("Login")]
        [ProducesResponseTypeWithDescription<AuthenticationToken>(StatusCodes.Status200OK, Description = "Authentication was successful.")]
        [ProducesResponseTypeWithDescription(StatusCodes.Status202Accepted, Description = "Authentication was successful, but 2fa is required.")]
        [ProducesResponseTypeWithDescription<ProblemDetails>(StatusCodes.Status401Unauthorized, Description = "The user does not exist or password is incorrect.")]
        [ProducesResponseTypeWithDescription<ProblemDetails>(StatusCodes.Status403Forbidden, Description = "The user account is locked.")]
        [Produces("application/json")]
        [OperationTransformer<LoginEndpointTransformer>]
        [EndpointDescription("An endpoint for user login. This endpoint handles authentication using a jwt access token and a randomly generated refresh token.")]
        [HttpPost("Login")]
        public async Task<ActionResult<AuthenticationToken>> Login([Description("Object containing username and password for login")] UserLoginDto userLogin)
        {
            LoginDto login = await _authentication.Login(userLogin);

            if (!login.Result.Succeeded && !login.Result.RequiresTwoFactor)
            {
                return Problem(
                    title: "Login failed",
                    detail: "The provided username or password is incorrect.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }
            else if (login.Result.RequiresTwoFactor)
            {
                return Accepted(new { 
                    Message = "Two factor authentication is required. Please check your email for code.",
                    login.UserId
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

        [EndpointSummary("Two factor login")]
        [ProducesResponseTypeWithDescription<ProblemDetails>(StatusCodes.Status401Unauthorized, Description = "The provided code is invalid or user doesn't exist.")]
        [ProducesResponseTypeWithDescription<ProblemDetails>(StatusCodes.Status403Forbidden, Description = "The user account is locked.")]
        [ProducesResponseTypeWithDescription<AuthenticationToken>(StatusCodes.Status200OK, Description = "Two factor authentication is successful.")]
        [Produces("application/json")]
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

        [EndpointSummary("Refresh tokens")]
        [ProducesResponseTypeWithDescription<AuthenticationToken>(StatusCodes.Status200OK, Description = "The token was successfully refreshed.")]
        [ProducesResponseTypeWithDescription(StatusCodes.Status401Unauthorized, Description = "Refresh or access token is invalid")]
        [Produces("application/json")]
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

        [EndpointSummary("Logout")]
        [ProducesResponseTypeWithDescription(StatusCodes.Status204NoContent, Description = "User successfully logged out.")]
        [ProducesResponseTypeWithDescription(StatusCodes.Status401Unauthorized, Description = "Access or refresh token is missing.")]
        [ProducesResponseTypeWithDescription<ProblemDetails>(StatusCodes.Status400BadRequest, Description = "User does not exist.")]
        [EndpointDescription("An endpoint for handling logout. This endpoint will delete the login session from db when successful.")]
        [Authorize]
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
