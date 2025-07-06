using ApplicationLogic.Interfaces;
using ApplicationLogic.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MovieReservation.Startup
{
    public static class ImagesFileEndpointExtensions
    {
        /// <summary>
        /// Maps an endpoint to retreive images from a source.
        /// </summary>
        /// <param name="endpoints">the endpoint route builder</param>
        public static void MapImages(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("api/Images/{fileName}", Results<FileStreamHttpResult, ProblemHttpResult> (string fileName, [FromServices] IFileHandler handler) =>
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
                    return TypedResults.Problem(
                        title: "File retrieval error",
                        detail: "This is not a supported file type.",
                        statusCode: StatusCodes.Status400BadRequest);
                }

                FileHandlerResult result = handler.GetFile(fileName);

                if (!result.Success || result.FileStream is null)
                {
                    return TypedResults.Problem(
                        title: "File retrieval error",
                        detail: "This file does not exist.",
                        statusCode: StatusCodes.Status404NotFound);
                }

                return TypedResults.File(result.FileStream, contentType, result.FileName);
            })
                .WithName("Images")
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status200OK);
        }
    }
}
