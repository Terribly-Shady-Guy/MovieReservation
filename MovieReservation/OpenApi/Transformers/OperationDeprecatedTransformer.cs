using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Asp.Versioning;

namespace MovieReservation.OpenApi.Transformers
{
    public class OperationDeprecatedTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            if (context.Description.IsDeprecated())
            {
                operation.Deprecated = true;

                SunsetPolicy? policy = context.Description.GetSunsetPolicy();
                operation.Description += $"\n> [!warning] This endpoint is deprecated and {(policy is not null ? $"will be removed on **{policy.Date}**" : "may be removed in a future version")}.";
            }

            return Task.CompletedTask;
        }
    }
}
