using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.OpenApiTransformers;
using MovieReservation.Services;
using MovieReservation.ViewModels;

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

        /// <summary>
        /// An endpoint to add a new user in the database.
        /// </summary>
        /// <param name="user">An object representing the new user.</param>
        /// <returns></returns>
        /// <response code="201">The new user account was sucessfully created.</response>
        [ProducesResponseTypeWithDescription(StatusCodes.Status201Created, Description = "The new user account was sucessfully created.")]
        [HttpPost]
        public async Task<ActionResult> AddNewUser(NewUserVM user)
        {
            string? id = await _userService.AddNewUserAsync(user);

            if (id is null)
            {
                return BadRequest();
            }

            return Created(id, new { Message = "Account created sucessfully." });
        }

        /// <summary>
        /// An endpoint for admin users to promote other users to admin role.
        /// </summary>
        /// <param name="id">The id for the promoted user</param>
        /// <returns></returns>
        /// <response code="200">The user was sucessfully promoted to admin.</response>
        /// <response code="404">The user with specified id does not exist.</response>
        [ProducesResponseTypeWithDescription(StatusCodes.Status200OK, Description = "The user was sucessfully promoted to admin.")]
        [ProducesResponseTypeWithDescription(StatusCodes.Status404NotFound, Description = "The user with specified id does not exist.")]
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public async Task<ActionResult> PromoteUser(string id)
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
