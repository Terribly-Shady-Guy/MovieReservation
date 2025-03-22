using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace MovieReservation.OpenApi.Transformers
{
    public class EndpointResponseDescriptionTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var responseTypes = context.Description.ActionDescriptor.EndpointMetadata
                .OfType<ProducesResponseTypeWithDescriptionAttribute>();

            foreach (var responseType in responseTypes)
            {
                bool isStatusCodeInResponses = operation.Responses.TryGetValue(
                    key: responseType.StatusCode.ToString(),
                    value: out OpenApiResponse? response);

                if (responseType.Description is not null && isStatusCodeInResponses && response is not null)
                {
                    response.Description = responseType.Description;
                }
            }

            return Task.CompletedTask;
        }
    }
}
