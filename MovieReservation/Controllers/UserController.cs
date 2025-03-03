using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.OpenApiTransformers;
using MovieReservation.Services;
using MovieReservation.ViewModels;
using System.ComponentModel;

namespace MovieReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [EndpointSummary("Add user")]
        [EndpointDescription("An endpoint to create a new user account with User role.")]
        [ProducesResponseTypeWithDescription(StatusCodes.Status201Created, Description = "The new user account was successfully created.")]
        [HttpPost]
        public async Task<ActionResult> AddNewUser([Description("An object containing new user info for account.")]NewUserVM user)
        {
            string? id = await _userService.AddNewUserAsync(user);

            if (id is null)
            {
                return BadRequest();
            }

            return Created(id, new { Message = "Account created sucessfully." });
        }

        [EndpointSummary("Promote user to admin")]
        [EndpointDescription("An endpoint that allows admin ussers to promote a user to Admin role.")]
        [ProducesResponseTypeWithDescription(StatusCodes.Status200OK, Description = "The user was successfully promoted to admin.")]
        [ProducesResponseTypeWithDescription(StatusCodes.Status404NotFound, Description = "The user with specified id does not exist.")]
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public async Task<ActionResult> PromoteUser([Description("The id for an existing user to promote.")] string id)
        {
            bool isSucessful = await _userService.PromoteToAdmin(id);

            if (!isSucessful)
            {
                return NotFound();
            }

            return Ok(new {Message = $"Sucessfully promoted user {id} to \"Admin\"."});
        }
    }
}
