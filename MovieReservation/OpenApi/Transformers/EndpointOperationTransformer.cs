using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace MovieReservation.OpenApi.Transformers
{
    public class EndpointOperationTransformer : IOpenApiOperationTransformer
    {
        private readonly Dictionary<string, IOpenApiOperationTransformer> _transformerCache = [];
        private readonly ILogger<EndpointOperationTransformer> _logger;

        public EndpointOperationTransformer(ILogger<EndpointOperationTransformer> logger)
        {
            _logger = logger;
        }

        public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Applying endpoint scoped operation transformers for {Operation}.", context.Description.ActionDescriptor.DisplayName);

            var endpointOperationTransformerMetadata = context.Description.ActionDescriptor.EndpointMetadata
                .OfType<IEndpointOperationTransformerMetadata>();
                
            foreach (var transformerMetadata in endpointOperationTransformerMetadata)
            {
                IOpenApiOperationTransformer transformer = GetOrCreateTransformer(context.ApplicationServices, transformerMetadata);
                await transformer.TransformAsync(operation, context, cancellationToken);
            }
        }

        private IOpenApiOperationTransformer GetOrCreateTransformer(IServiceProvider serviceProvider, IEndpointOperationTransformerMetadata metadata)
        {
            string cacheKey = metadata.TransformerTypeName;

            if (_transformerCache.TryGetValue(cacheKey, out IOpenApiOperationTransformer? transformer))
            {
                return transformer;
            }

            transformer = metadata.CreateTransformer(serviceProvider);

            _transformerCache.Add(cacheKey, transformer);
            return transformer;
        }
    }
}
