using ApplicationLogic.Interfaces;
using ApplicationLogic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.OpenApi;

namespace MovieReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IFileHandler _fileHandler;

        public ImageController(IFileHandler fileHandler)
        {
            _fileHandler = fileHandler;
        }

        [EndpointSummary("Get Images")]
        [EndpointDescription("Gets the image based on the provided file name in the url.")]
        [ProducesResponseTypeWithDescription(StatusCodes.Status200OK, Description = "The file was successfully located.")]
        [ProducesResponseTypeWithDescription<ProblemDetails>(StatusCodes.Status400BadRequest, Description = "The file type is not supported.")]
        [ProducesResponseTypeWithDescription<ProblemDetails>(StatusCodes.Status404NotFound, Description = "The file does not exist.")]
        [HttpGet("{fileName}")]
        public ActionResult GetImage(string fileName)
        {
            var contentTypes = new Dictionary<string, string>
            {
                [".jpg"] = "image/jpeg",
                [".jpeg"] = "image/jpeg",
                [".png"] = "image/png"
            };

            string? ext = Path.GetExtension(fileName);

            if (ext is null || !contentTypes.TryGetValue(ext, out string? contentType))
            {
                return Problem(
                    title: "File retrieval error",
                    detail: "This is not a supported file type.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            FileHandlerResult result = _fileHandler.GetFile(fileName);

            if (!result.Success)
            {
                return Problem(
                    title: "File retrieval error",
                    detail: "This file does not exist.",
                    statusCode: StatusCodes.Status404NotFound);
            }

            return File(result.FileStream, contentType, result.FileName);
        }
    }
}
