using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace MovieReservation.OpenApi.Transformers
{
    public class NewUserEndpointTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var requestExample = new OpenApiObject()
            {
                ["username"] = new OpenApiString("johndoe"),
                ["password"] = new OpenApiPassword("Password123@"),
                ["firstName"] = new OpenApiString("John"),
                ["lastName"] = new OpenApiString("Doe"),
                ["email"] = new OpenApiString("john.doe@example.com")
            };

            foreach (var content in operation.RequestBody.Content)
            {
                content.Value.Example = requestExample;
            }

            return Task.CompletedTask;
        }
    }
}
