using ApplicationLogic.Interfaces;
using ApplicationLogic.ViewModels;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace MovieReservation.Controllers
{
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IFileHandler _fileHandler;

        public ImageController(IFileHandler fileHandler)
        {
            _fileHandler = fileHandler;
        }

        [MapToApiVersion(1.0)]
        [EndpointSummary("Get Image")]
        [EndpointDescription("Gets the image based on the provided file name in the url.")]
        [Produces(MediaTypeNames.Image.Jpeg, MediaTypeNames.Image.Png)]
        [ProducesResponseType(StatusCodes.Status200OK, Description = "The file was successfully located.")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, Description = "The file type is not supported.")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, Description = "The file does not exist.")]
        [HttpGet("{fileName}")]
        public ActionResult GetImage(string fileName)
        {
            var contentTypes = new Dictionary<string, string>
            {
                [".jpg"] = MediaTypeNames.Image.Jpeg,
                [".jpeg"] = MediaTypeNames.Image.Jpeg,
                [".png"] = MediaTypeNames.Image.Png
            };

            string? ext = Path.GetExtension(fileName);

            if (ext is null || !contentTypes.TryGetValue(ext, out string? contentType))
            {
                return Problem(
                    title: "File retrieval error",
                    detail: "This is not a supported file type.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            Result<Stream> result = _fileHandler.GetFile(fileName);

            if (result.Failure)
            {
                return Problem(
                    title: "File retrieval error",
                    detail: "This file does not exist.",
                    statusCode: StatusCodes.Status404NotFound);
            }

            return File(result.Value, contentType);
        }
    }
}
