using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace MovieReservation.OpenApi.Transformers
{
    /// <summary>
    /// An operation transformer to enable endpoint scoped operation transformations.
    /// </summary>
    public class EndpointOperationTransformer : IOpenApiOperationTransformer
    {
        private readonly ILogger<EndpointOperationTransformer> _logger;

        public EndpointOperationTransformer(ILogger<EndpointOperationTransformer> logger)
        {
            _logger = logger;
        }

        public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var endpointOperationTransformerMetadata = context.Description.ActionDescriptor.EndpointMetadata
                .OfType<IEndpointOperationTransformerMetadata>();
                
            foreach (var transformerMetadata in endpointOperationTransformerMetadata)
            {
                try
                {
                    var transformer = (IOpenApiOperationTransformer)ActivatorUtilities.CreateInstance(
                        provider: context.ApplicationServices,
                        instanceType: transformerMetadata.TransformerType);

                    await transformer.TransformAsync(operation, context, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "The transformer {TransformerName} could not be created or applied.", transformerMetadata.TransformerType.Name);
                }
            }
        }
    }
}
