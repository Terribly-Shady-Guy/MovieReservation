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

        /// <summary>
        /// An endpoint for retrieving list of movies.
        /// </summary>
        /// <param name="genre">Optional filter by movie genre.</param>
        /// <returns>The list of available movies.</returns>
        /// <response code="200">Sucessfully retrieves list of movies.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult<List<MovieVM>>> ListMovies([FromQuery] string? genre)
        {
            return Ok(await _movieService.GetMovies(genre));
        }

        /// <summary>
        /// An endpoint for admins to add a new movie.
        /// </summary>
        /// <param name="movie">A formdata object representing the new movie.</param>
        /// <returns></returns>
        /// <response code="201">Movie was sucessfully added.</response>
        /// <remarks>
        /// The uploaded poster image file must meet the following requirements:
        /// Type must be either a .jpg, .jpeg, or .png.
        /// Size must be 10mb or smaller.
        /// </remarks>
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

        /// <summary>
        /// An endpoint for an admin to delete a movie.
        /// </summary>
        /// <param name="id">The movie id</param>
        /// <returns></returns>
        /// <response code="204">The movie was deleted sucessfully.</response>
        /// <response code="404">The id for the movie does not exist.</response>
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
