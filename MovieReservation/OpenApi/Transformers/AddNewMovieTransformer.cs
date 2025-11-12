using Microsoft.AspNetCore.OpenApi;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi;

namespace MovieReservation.OpenApi.Transformers
{
    public class AddNewMovieTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            if (operation.Responses is not null && operation.Responses.TryGetValue(StatusCodes.Status201Created.ToString(), out IOpenApiResponse? response))
            {
                response.Headers?.Add(HeaderNames.Location, new OpenApiHeader
                {
                    Description = "A header containing the uri for retrieving the new resource.",
                    Schema = new OpenApiSchema 
                    { 
                        Format = "uri",
                        Type = JsonSchemaType.String
                    }
                });
            }

            return Task.CompletedTask;
        }
    }
}
