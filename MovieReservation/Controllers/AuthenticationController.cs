using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.Models;
using MovieReservation.Services;
using MovieReservation.ViewModels;

namespace MovieReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtManager _manager;

        public AuthenticationController(UserService userService, JwtManager manager)
        {
            _userService = userService;
            _manager = manager;
        }

        [HttpPost]
        public async Task<ActionResult> Login(UserLoginVM userLogin)
        {
            AppUser? user = await _userService.GetUserAsync(userLogin);

            if (user == null)
            {
                return Unauthorized(new {Message = $"User {userLogin.Username} does not exist."});
            }

            string token = _manager.GenerateJwt(user);

            return Ok(new { Token = token });
        }
    }
}
