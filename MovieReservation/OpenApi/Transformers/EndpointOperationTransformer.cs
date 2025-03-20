using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace MovieReservation.OpenApi.Transformers
{
    public class EndpointOperationTransformer : IOpenApiOperationTransformer
    {
        private readonly Dictionary<string, IOpenApiOperationTransformer> _transformerCache = [];

        public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var endpointOperationTransformerMetadata = context.Description.ActionDescriptor.EndpointMetadata
                .OfType<IEndpointOperationTransformerMetadata>();
                
            foreach (var transformerMetadata in endpointOperationTransformerMetadata)
            {
                IOpenApiOperationTransformer? transformer = GetOrCreateTransformer(context.ApplicationServices, transformerMetadata);
                if (transformer is not null)
                {
                    await transformer.TransformAsync(operation, context, cancellationToken);
                }
            }
        }

        private IOpenApiOperationTransformer? GetOrCreateTransformer(IServiceProvider serviceProvider, IEndpointOperationTransformerMetadata metadata)
        {
            string cacheKey = metadata.TransformerType.FullName ?? metadata.TransformerType.Name;
            IOpenApiOperationTransformer? transformer = null;

            if (_transformerCache.TryGetValue(cacheKey, out transformer))
            {
                return transformer;
            }

            transformer = metadata.CreateTransformer(serviceProvider);
            if (transformer == null)
            {
                return null;
            }

            _transformerCache.Add(cacheKey, transformer);
            return transformer;
        }
    }
}
