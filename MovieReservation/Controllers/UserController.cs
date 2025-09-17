using ApplicationLogic;
using ApplicationLogic.Services;
using ApplicationLogic.ViewModels;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.OpenApi;
using MovieReservation.OpenApi.Transformers;
using System.ComponentModel;
using System.Security.Claims;

namespace MovieReservation.Controllers
{
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [MapToApiVersion(1.0)]
        [EndpointSummary("Add user")]
        [EndpointDescription("An endpoint to create a new user account with User role.")]
        [ProducesResponseTypeWithDescription(StatusCodes.Status201Created, Description = "The new user account was successfully created.")]
        [OperationTransformer<NewUserEndpointTransformer>]
        [HttpPost("Create")]
        public async Task<ActionResult> AddNewUser([Description("An object containing new user info for account.")] NewUserDto user)
        {
            Result<string> userResult = await _userService.AddNewUserAsync(user);

            if (userResult.Failure)
            {
                return Problem(
                    title: "Bad Request",
                    detail: "The account could not be created.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            return StatusCode(StatusCodes.Status201Created, new { Message = "Account created successfully. A confirmation email has been sent. Please verify your email to activate your account." });
        }

        [MapToApiVersion(1.0)]
        [Authorize]
        [HttpPost("Change-Password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordBody passwordBody)
        {
            try
            {
                string userId = User.Claims
                    .Single(claim => claim.Type == ClaimTypes.NameIdentifier)
                    .Value;

                return Ok();
            }
            catch (InvalidOperationException)
            {
                return Problem(
                    title: "Invalid Token",
                    detail: "The provided access token is invalid.",
                    statusCode: StatusCodes.Status400BadRequest);
            }
        }

        [MapToApiVersion(1.0)]
        [HttpPost("{id}/Email-Confirmation")]
        public async Task<ActionResult> ConfirmEmail(string id, string token)
        {
            bool result = await _userService.ConfirmEmail(id, token);
            if (!result)
            {
                return Problem(
                    title: "Confirmation Failed",
                    detail: "The provided email token is invalid.",
                    statusCode: StatusCodes.Status400BadRequest);
            }
            
            return Ok(new { Message = "Your email has been successfully verified. Your account is now active." });
        }

        [MapToApiVersion(1.0)]
        [EndpointSummary("Promote user to admin")]
        [EndpointDescription("An endpoint that allows admin users to promote a user to Admin role.")]
        [ProducesResponseTypeWithDescription(StatusCodes.Status200OK, Description = "The user was successfully promoted to admin.")]
        [ProducesResponseTypeWithDescription(StatusCodes.Status404NotFound, Description = "The user with specified id does not exist.")]
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpPatch("{id}")]
        public async Task<ActionResult> PromoteUser([Description("The id for an existing user to promote.")] string id)
        {
            Result promotionResult = await _userService.PromoteToAdmin(id);

            if (promotionResult.Failure)
            {
                return Problem(
                    title: "User Not Found",
                    detail: "The user with the provided id could not be found.",
                    statusCode: StatusCodes.Status404NotFound);
            }

            return Ok(new {Message = $"Successfully promoted user {id} to \"{Roles.Admin}\"."});
        }
    }
}
