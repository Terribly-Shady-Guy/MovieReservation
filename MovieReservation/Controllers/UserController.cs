using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost]
        public async Task<ActionResult> AddNewUser(NewUserVM user)
        {
            string? id = await _userService.AddNewUserAsync(user);

            if (id is null)
            {
                return BadRequest();
            }

            return Created(id.ToString(), new { Mesaage = "Account created sucessfully." });
        }

        /// <summary>
        /// An endpoint for admin users to promote other users to admin role.
        /// </summary>
        /// <param name="id">The id for the promoted user</param>
        /// <returns></returns>
        /// <response code="200">The user was sucessfully promoted to admin.</response>
        /// <response code="404">The user with specified id does not exist.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
