using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace MovieReservation.OpenApiTransformers
{
    public class ResponseTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var responseTypes = context.Description.ActionDescriptor.EndpointMetadata
                .OfType<ProducesResponseTypeWithDescriptionAttribute>();

            foreach (var responseType in responseTypes)
            {
                if (responseType.Description is not null && operation.Responses.TryGetValue(responseType.StatusCode.ToString(), out OpenApiResponse? response))
                {
                    response.Description = responseType.Description;
                }
            }

            return Task.CompletedTask;
        }
    }
}
