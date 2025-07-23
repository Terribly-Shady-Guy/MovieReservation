using ApplicationLogic.Services;
using ApplicationLogic.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.Startup;

namespace MovieReservation.Controllers
{
    [Authorize(Policy = RegisteredAuthorizationPolicyNames.IsAdmin)]
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly LocationService _locationService;

        public LocationController(LocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpPost]
        public async Task<ActionResult> AddLocation(LocationDto location)
        {
            await _locationService.AddLocation(location);
            return CreatedAtAction("AddLocation", new { Message = "New location added. " });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLocation(int id)
        {
            await _locationService.DeleteLocation(id);
            return NoContent();
        }
    }
}
