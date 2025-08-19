using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.Startup;

namespace MovieReservation.Controllers
{
    [ApiVersion(1.0)]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ShowingController : ControllerBase
    {
        [MapToApiVersion(1.0)]
        [HttpGet("{movieId}")]
        public async Task<ActionResult> GetShowings(int movieId)
        {
            throw new NotImplementedException();
        }

        [MapToApiVersion(1.0)]
        [Authorize(Policy = RegisteredAuthorizationPolicyNames.IsAdmin)]
        [HttpPost]
        public async Task<ActionResult> AddShowing()
        {
            throw new NotImplementedException();
        }

        [MapToApiVersion(1.0)]
        [Authorize(Policy = RegisteredAuthorizationPolicyNames.IsAdmin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteShowing(int id)
        {
            throw new NotImplementedException();
        }
    }
}
