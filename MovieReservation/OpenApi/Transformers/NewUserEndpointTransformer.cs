using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Text.Json.Nodes;


namespace MovieReservation.OpenApi.Transformers
{
    public class NewUserEndpointTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            if (operation.RequestBody? .Content == null)
            {
                return Task.CompletedTask;
            }
            var requestExample = new JsonObject()
            {
                ["username"] = "johndoe",
                ["password"] = "Password123@",
                ["firstName"] = "John",
                ["lastName"] = "Doe",
                ["email"] = "john.doe@example.com"
            };

            foreach (var content in operation.RequestBody.Content)
            {
                content.Value.Example = requestExample;
            }

            return Task.CompletedTask;
        }
    }
}
