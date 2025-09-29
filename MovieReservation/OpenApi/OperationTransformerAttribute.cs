using Microsoft.AspNetCore.OpenApi;

namespace MovieReservation.OpenApi
{
    /// <summary>
    /// An attribute to apply operation transformers to specific endpoint methods or controllers.
    /// </summary>
    /// <typeparam name="TOperationTransformer">The operation transformer to apply.</typeparam>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class OperationTransformerAttribute<TOperationTransformer> : Attribute, IEndpointOperationTransformerMetadata
        where TOperationTransformer : IOpenApiOperationTransformer
    {
        public Type TransformerType { get; } = typeof(TOperationTransformer);
    }
}
