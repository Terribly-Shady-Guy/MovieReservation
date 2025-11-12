using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
namespace MovieReservation.OpenApi.Transformers
{
    /// <summary>
    /// An operation transformer to enable endpoint scoped operation transformations.
    /// </summary>
    /// <remarks>This transformer will skip executing a transformer if an error occurs. It will log the issue when it skips instead.</remarks>
    public class EndpointOperationTransformer : IOpenApiOperationTransformer
    {
        private readonly ILogger<EndpointOperationTransformer> _logger;

        public EndpointOperationTransformer(ILogger<EndpointOperationTransformer> logger)
        {
            _logger = logger;
        }

        public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var endpointTransformerMetadata = context.Description.ActionDescriptor.EndpointMetadata
                .OfType<IEndpointOperationTransformerMetadata>();
                
            foreach (var transformerMetadataItem in endpointTransformerMetadata)
            {
                try
                {
                    var endpointTransformer = (IOpenApiOperationTransformer)ActivatorUtilities.CreateInstance(
                        provider: context.ApplicationServices,
                        instanceType: transformerMetadataItem.TransformerType);

                    await endpointTransformer.TransformAsync(operation, context, cancellationToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "The transformer {TransformerName} for action {ActionName} was skipped due to an error.", 
                        transformerMetadataItem.TransformerType.Name, 
                        context.Description.ActionDescriptor.DisplayName);
                }
            }
        }
    }
}
