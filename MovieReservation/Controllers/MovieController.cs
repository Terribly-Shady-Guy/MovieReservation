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
        private readonly ILogger<MovieController> _logger;

        public MovieController(MovieService movieService, LinkGenerator link, ILogger<MovieController> logger)
        {
            _movieService = movieService;
            _linkGenerator = link;
            _logger = logger;
        }

        [EndpointSummary("List movies")]
        [EndpointDescription("An endpoint that allows logged in users to retrieve list of movies optionally filtered by genre.")]
        [ProducesResponseTypeWithDescription<List<MovieListItem>>(StatusCodes.Status200OK, Description = "Successfully retrieved the list of movies")]
        [Produces("application/json")]
        [HttpGet]
        public async Task<ActionResult<List<MovieListItem>>> ListMovies([FromQuery, Description("Optional parameter to filter by genre.")] string? genre)
        {
            List<MovieListItem> movies = await _movieService.GetMovies(genre);

            return Ok(movies);
        }

        [EndpointSummary("Get movie by id")]
        [EndpointDescription("Gets a movie by id and either returns null if it does not exist or the movie with a link to get its image. Use the posterImageName property if imageLink is not available.")]
        [ProducesResponseTypeWithDescription<MovieVM>(StatusCodes.Status200OK, Description = "The object containing the movie info.")]
        [ProducesResponseTypeWithDescription(StatusCodes.Status404NotFound, Description = "The movie with the provided id doesn't exist.")]
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieVM>> GetMovie(int id)
        {
            MovieVM? movie = await _movieService.GetById(id);

            if (movie is null)
            {
                return Problem(
                    title: "Movie not found",
                    detail: $"The movie with id {id} does not exist.",
                    statusCode: StatusCodes.Status404NotFound);
            }

            string imagesControllerName = nameof(ImagesController)
                .Replace("Controller", string.Empty);

            string getImageActionName = nameof(ImagesController.GetImage);

            string? link = _linkGenerator.GetUriByAction(
                httpContext: HttpContext,
                controller: imagesControllerName,
                action: getImageActionName,
                values: new { fileName = movie.PosterImageName });

            if (link is null)
            {
                _logger.LogWarning("The link using {ControllerName} and {ActionName} could not be created.", imagesControllerName, imagesControllerName);
                return Ok(movie);
            }

            movie.ImageLink = link;

            return Ok(movie);
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
           
            return CreatedAtAction(nameof(GetMovie), new { Message = "New movie added" });
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
                    detail: "The movie could not be deleted.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            return NoContent();
        }
    }
}
