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

        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost]
        public async Task<ActionResult> AddNewUser(NewUserVM user)
        {
            int id = await _userService.AddNewUserAsync(user);

            return Created(id.ToString(), new { Mesaage = "Account created sucessfully." });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> PromoteUser(int id)
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
