using ApplicationLogic;
using ApplicationLogic.Services;
using ApplicationLogic.ViewModels;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.OpenApi;
using MovieReservation.OpenApi.Transformers;
using System.ComponentModel;

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
        [HttpPost("Add")]
        public async Task<ActionResult> AddNewUser([Description("An object containing new user info for account.")] NewUserDto user)
        {
            string? id = await _userService.AddNewUserAsync(user);

            if (id is null)
            {
                return Problem(
                    title: "Bad Request",
                    detail: "The account could not be created.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            return Created(id, new { Message = "Account created sucessfully." });
        }

        [MapToApiVersion(1.0)]
        [Authorize]
        [HttpPost("Change-Password")]
        public async Task<ActionResult> ChangePassword()
        {
            throw new NotImplementedException();
        }

        [MapToApiVersion(1.0)]
        [HttpPost("Confirm-Email")]
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

            return Ok(new {Message = "Email is confirmed."});
        }

        [MapToApiVersion(1.0)]
        [EndpointSummary("Promote user to admin")]
        [EndpointDescription("An endpoint that allows admin ussers to promote a user to Admin role.")]
        [ProducesResponseTypeWithDescription(StatusCodes.Status200OK, Description = "The user was successfully promoted to admin.")]
        [ProducesResponseTypeWithDescription(StatusCodes.Status404NotFound, Description = "The user with specified id does not exist.")]
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpPatch("{id}")]
        public async Task<ActionResult> PromoteUser([Description("The id for an existing user to promote.")] string id)
        {
            bool isSucessful = await _userService.PromoteToAdmin(id);

            if (!isSucessful)
            {
                return Problem(
                    title: "Bad Request",
                    detail: "The user with the provided id could not be found.",
                    statusCode: StatusCodes.Status404NotFound);
            }

            return Ok(new {Message = $"Sucessfully promoted user {id} to \"Admin\"."});
        }
    }
}
