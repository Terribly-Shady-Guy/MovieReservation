using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace MovieReservation.Controllers
{
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuditoriumController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> AddNewAuditoriumToLocation()
        {
            throw new NotImplementedException();
        }
    }
}
