using ApplicationLogic.Services;
using ApplicationLogic.ViewModels;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.Startup;

namespace MovieReservation.Controllers
{
    [Authorize(Policy = RegisteredAuthorizationPolicyNames.IsAdmin)]
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly LocationService _locationService;

        public LocationController(LocationService locationService)
        {
            _locationService = locationService;
        }

        [MapToApiVersion(1.0)]
        [HttpPost]
        public async Task<ActionResult<ResponseMessage>> AddLocation(LocationDto location)
        {
            await _locationService.AddLocation(location);
            return CreatedAtAction("AddLocation", new ResponseMessage { Message = "New location added. " });
        }

        [MapToApiVersion(1.0)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLocation(int id)
        {
            await _locationService.DeleteLocation(id);
            return NoContent();
        }
    }
}
