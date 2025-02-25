using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DbInfrastructure.Services;
using DbInfrastructure.ViewModels;

namespace DbInfrastructure.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public async Task<ActionResult> AddLocation(LocationVM location)
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
