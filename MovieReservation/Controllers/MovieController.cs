using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.Services;
using MovieReservation.ViewModels;

namespace MovieReservation.Controllers
{
    [Authorize(Roles = "Admin, User")]
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieService _movieService;

        public MovieController(MovieService movieService)
        {
            _movieService = movieService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult<MovieVM>> ListMovies([FromQuery] string? genre)
        {
            return Ok(await _movieService.GetMovies(genre));
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddNewMovie([FromForm] MovieFormDataBody movie)
        {
            await _movieService.AddMovie(movie);

            return CreatedAtAction("AddNewMovie", new { Message = "New movie added" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult> EditMovie()
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMovie(int id)
        {
            bool isDeleted = await _movieService.DeleteMovie(id);

            if (!isDeleted)
            {
                return BadRequest(new { Message = $"Failed to delete user id {id}" });
            }

            return NoContent();
        }
    }
}
