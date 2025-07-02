using ApplicationLogic.Services;
using ApplicationLogic.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.OpenApi;
using MovieReservation.Startup;
using System.ComponentModel;

namespace MovieReservation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieService _movieService;
        private readonly LinkGenerator _linkGenerator;

        public MovieController(MovieService movieService, LinkGenerator link)
        {
            _movieService = movieService;
            _linkGenerator = link;
        }

        [EndpointSummary("List movies")]
        [EndpointDescription("An endpoint that allows logged in users to retrieve list of movies optionally filtered by gemre.")]
        [ProducesResponseTypeWithDescription<List<MovieVM>>(StatusCodes.Status200OK, Description = "Successfully retrieved the list of movies")]
        [Produces("application/json")]
        [HttpGet]
        public async Task<ActionResult<List<MovieVM>>> ListMovies([FromQuery, Description("Optional parameter to filter by genre.")] string? genre)
        {
            List<MovieVM> movies = await _movieService.GetMovies(genre);

            foreach (MovieVM movie in movies)
            {
                movie.PosterImageName = _linkGenerator.GetPathByName(HttpContext, "Images", movie.PosterImageName) ?? movie.PosterImageName;
            }
            
            return Ok(movies);
        }

        [EndpointSummary("Add new movie.")]
        [EndpointDescription("An endpoint that allows admins to add a new movie. The uploaded movie poster file must meet the following requirements: Type must beeither a jpg, jpeg, or png. Size must be 10 mb or smaller.")]
        [Consumes("multipart/form-data")]
        [ProducesResponseTypeWithDescription(StatusCodes.Status201Created, Description = "Movie was successfully added.")]
        [Authorize(Policy = RegisteredAuthorizationPolicyNames.IsAdmin)]
        [HttpPost]
        public async Task<ActionResult> AddNewMovie([FromForm, Description("Formdata object containing input for new movie.")] MovieFormDataBody movie)
        {
            await _movieService.AddMovie(movie);

            return CreatedAtAction("AddNewMovie", new { Message = "New movie added" });
        }

        [Authorize(Policy = RegisteredAuthorizationPolicyNames.IsAdmin)]
        [HttpPut]
        public async Task<ActionResult> EditMovie()
        {
            throw new NotImplementedException();
        }

        [EndpointSummary("Delete movie")]
        [EndpointDescription("An endpoint that allows an admin user to delete a movie.")]
        [ProducesResponseTypeWithDescription(StatusCodes.Status204NoContent, Description = "The movie was successfully deleted.")]
        [ProducesResponseTypeWithDescription(StatusCodes.Status400BadRequest, Description = "The id for the move does not exist.")]
        [Authorize(Policy = RegisteredAuthorizationPolicyNames.IsAdmin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMovie([Description("The movie id.")] int id)
        {
            bool isDeleted = await _movieService.DeleteMovie(id);

            if (!isDeleted)
            {
                return Problem(
                    title: "Bad Request",
                    detail: "The user could not be deleted.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            return NoContent();
        }
    }
}
