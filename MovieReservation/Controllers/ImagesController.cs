using ApplicationLogic.Interfaces;
using ApplicationLogic.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MovieReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IFileHandler _fileHandler;

        public ImagesController(IFileHandler fileHandler)
        {
            _fileHandler = fileHandler;
        }

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

            if (!result.Success || result.FileStream is null)
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
