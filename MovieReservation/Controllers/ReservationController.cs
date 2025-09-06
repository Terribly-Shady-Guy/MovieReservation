using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MovieReservation.Controllers
{
    [ApiVersion(1.0)]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {

    }
}
