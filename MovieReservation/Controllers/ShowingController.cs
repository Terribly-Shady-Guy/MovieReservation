using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MovieReservation.Controllers
{
    [Authorize(Roles = "User, Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ShowingController : ControllerBase
    {
        [HttpGet("{movieId}")]
        public async Task<ActionResult> GetShowings(int movieId)
        {
            throw new NotImplementedException();
        }

        [Authorize(Policy = "IsAdmin")]
        [HttpPost]
        public async Task<ActionResult> AddShowing()
        {
            throw new NotImplementedException();
        }

        [Authorize(Policy = "IsAdmin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteShowing(int id)
        {
            throw new NotImplementedException();
        }
    }
}
