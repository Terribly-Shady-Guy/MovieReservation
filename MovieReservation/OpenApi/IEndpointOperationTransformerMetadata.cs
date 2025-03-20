using Microsoft.AspNetCore.OpenApi;

namespace MovieReservation.OpenApi
{
    public interface IEndpointOperationTransformerMetadata
    {
        public Type TransformerType { get; }
        public IOpenApiOperationTransformer? CreateTransformer(IServiceProvider services);
    }
}
