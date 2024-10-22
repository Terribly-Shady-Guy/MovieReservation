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

        [HttpGet]
        public async Task<ActionResult<MovieVM>> ListMovies([FromQuery] string? genre)
        {
            return Ok(await _movieService.GetMovies(genre));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddNewMovie([FromForm] MovieUploadVM movie)
        {
            string extension = Path.GetExtension(movie.PosterImage.FileName).ToLowerInvariant();

            string[] validExtensions = [".jpg", ".png", ".jpeg"];

            if (!validExtensions.Any(ext => ext == extension))
            {
                return BadRequest(new { Message = $"This is not a valid file type. File type must be one of the following: {string.Join(", ", validExtensions)}." });
            }

            long imageSizeInMB = movie.PosterImage.Length / (1024 * 1024);
            const int FileSizeLimitInMB = 10;

            if (imageSizeInMB > FileSizeLimitInMB)
            {
                return BadRequest(new { Message = $"The uploaded file must be {FileSizeLimitInMB}mb or smaller." });
            }

            await _movieService.AddMovie(movie);

            return CreatedAtAction("AddnewMovie", "New movie added");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult> EditMovie()
        {
            throw new NotImplementedException();
        }

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
