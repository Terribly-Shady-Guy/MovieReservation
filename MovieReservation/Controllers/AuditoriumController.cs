using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MovieReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditoriumController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> AddSeatToAuditorium(int auditoriumId)
        {
            throw new NotImplementedException();
        }
    }
}
