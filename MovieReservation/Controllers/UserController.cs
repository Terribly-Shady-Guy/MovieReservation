using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.Services;
using MovieReservation.ViewModels;
using System.Text.RegularExpressions;

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

        [HttpPost]
        public async Task<ActionResult> AddNewUser(NewUserVM user)
        {
            if (!Regex.IsMatch(user.Email, "[a-zA-Z0-9]+@[a-z]+[.]com"))
            {
                return BadRequest(new {Message = "email is not a valid format."});
            }

            int id = await _userService.AddNewUserAsync(user);

            return Created(id.ToString(), new { Mesaage = "Account created sucessfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
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
