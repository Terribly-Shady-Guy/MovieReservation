using Microsoft.AspNetCore.OpenApi;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace MovieReservation.OpenApi.Transformers
{
    public class AddNewMovieTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            if (operation.Responses.TryGetValue(StatusCodes.Status201Created.ToString(), out OpenApiResponse? response))
            {
                response.Headers.Add(HeaderNames.Location, new OpenApiHeader
                {
                    Description = "A header containing the uri for retrieving the new resource.",
                    Schema = new OpenApiSchema 
                    { 
                        Format = "uri",
                        Type = "string"
                    }
                });
            }

            return Task.CompletedTask;
        }
    }
}
