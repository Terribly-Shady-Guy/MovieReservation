using Microsoft.AspNetCore.OpenApi;

namespace MovieReservation.OpenApi
{
    public interface IEndpointOperationTransformerMetadata
    {
        public string TransformerTypeName { get; }
        public IOpenApiOperationTransformer? CreateTransformer(IServiceProvider services);
    }
}
