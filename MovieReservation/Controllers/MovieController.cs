using ApplicationLogic.Services;
using ApplicationLogic.ViewModels;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.OpenApi;
using MovieReservation.OpenApi.Transformers;
using MovieReservation.Startup;
using System.ComponentModel;
using System.Net.Mime;

namespace MovieReservation.Controllers
{
    [ApiVersion(1.0)]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
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

        [MapToApiVersion(1.0)]
        [EndpointSummary("List movies")]
        [EndpointDescription("An endpoint that allows logged in users to retrieve list of movies optionally filtered by genre.")]
        [ProducesResponseTypeWithDescription<List<MovieListItem>>(StatusCodes.Status200OK, Description = "Successfully retrieved the list of movies")]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet]
        public async Task<ActionResult<List<MovieListItem>>> GetMovieList([FromQuery, Description("Optional parameter to filter by genre.")] string? genre)
        {
            List<MovieListItem> movies = await _movieService.GetMovies(genre);

            return Ok(movies);
        }

        [MapToApiVersion(1.0)]
        [EndpointSummary("Get movie by id")]
        [EndpointDescription("Gets a movie by id and either returns null if it does not exist or the movie with a link to get its image. Use the posterImageName property if imageLink is not available.")]
        [ProducesResponseTypeWithDescription<MovieDto>(StatusCodes.Status200OK, Description = "The object containing the movie info.")]
        [ProducesResponseTypeWithDescription<ProblemDetails>(StatusCodes.Status404NotFound, Description = "The movie with the provided id doesn't exist.")]
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieDto>> GetMovie(int id)
        {
            MovieDto? movie = await _movieService.GetById(id);

            if (movie is null)
            {
                return Problem(
                    title: "Movie not found",
                    detail: $"The movie with id {id} does not exist.",
                    statusCode: StatusCodes.Status404NotFound);
            }

            string imagesControllerName = nameof(ImageController)
                .Replace("Controller", string.Empty);

            string getImageActionName = nameof(ImageController.GetImage);

            string? imageLink = _linkGenerator.GetUriByAction(
                httpContext: HttpContext,
                controller: imagesControllerName,
                action: getImageActionName,
                values: new { fileName = movie.ImageFileName });

            if (imageLink is null)
            {
                _logger.LogWarning("The image link using {ControllerName} and {ActionName} with id {Id} could not be created.", imagesControllerName, getImageActionName, id);
                return Ok(movie);
            }

            movie.ImageLink = imageLink;

            return Ok(movie);
        }

        [MapToApiVersion(1.0)]
        [EndpointSummary("Add new movie.")]
        [EndpointDescription("An endpoint that allows admins to add a new movie. The uploaded movie poster file must meet the following requirements: Type must beeither a jpg, jpeg, or png. Size must be 10 mb or smaller.")]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [OperationTransformer<AddNewMovieTransformer>]
        [ProducesResponseTypeWithDescription(StatusCodes.Status201Created, Description = "Movie was successfully added.")]
        [Authorize(Policy = RegisteredAuthorizationPolicyNames.IsAdmin)]
        [HttpPost]
        public async Task<ActionResult> AddNewMovie([FromForm, Description("Formdata object containing input for new movie.")] MovieFormDataBody movie)
        {
            int movieId = await _movieService.AddMovie(movie);

            return CreatedAtAction(
                actionName: nameof(GetMovie),
                routeValues: new { id = movieId }, 
                value: new { Message = "New movie added" });
        }

        [MapToApiVersion(1.0)]
        [Authorize(Policy = RegisteredAuthorizationPolicyNames.IsAdmin)]
        [HttpPut]
        public async Task<ActionResult> EditMovie()
        {
            throw new NotImplementedException();
        }

        [MapToApiVersion(1.0)]
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
