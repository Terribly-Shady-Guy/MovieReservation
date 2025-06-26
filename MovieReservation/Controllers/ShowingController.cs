using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.Startup;

namespace MovieReservation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShowingController : ControllerBase
    {
        [HttpGet("{movieId}")]
        public async Task<ActionResult> GetShowings(int movieId)
        {
            throw new NotImplementedException();
        }

        [Authorize(Policy = RegisteredAuthorizationPolicyNames.IsAdmin)]
        [HttpPost]
        public async Task<ActionResult> AddShowing()
        {
            throw new NotImplementedException();
        }

        [Authorize(Policy = RegisteredAuthorizationPolicyNames.IsAdmin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteShowing(int id)
        {
            throw new NotImplementedException();
        }
    }
}
