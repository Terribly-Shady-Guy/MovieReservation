using Microsoft.AspNetCore.OpenApi;
using System.Reflection;

namespace MovieReservation.OpenApi
{
    /// <summary>
    /// An attribute to apply operation transformers to specific endpoints or controllers.
    /// </summary>
    /// <typeparam name="TOperationTransformer">The operation transformer to apply.</typeparam>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class OperationTransformerAttribute<TOperationTransformer> : Attribute, IEndpointOperationTransformerMetadata
        where TOperationTransformer : IOpenApiOperationTransformer
    {
        private readonly Type _transformerType = typeof(TOperationTransformer);
        public Type TransformerType => _transformerType;

        public IOpenApiOperationTransformer? CreateTransformer(IServiceProvider services)
        {
            ConstructorInfo[] constructors = _transformerType.GetConstructors();

            ConstructorInfo? constructorToCall = null;
            ParameterInfo[] constructorParams = [];

            foreach (var constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                if (parameters.Length > constructorParams.Length || constructorToCall is null)
                {
                    constructorParams = parameters;
                    constructorToCall = constructor;
                }
            }
            
            if (constructorToCall is null) return null;

            var constructorArgs = new object[constructorParams.Length];

            for (int i = 0; i < constructorParams.Length; i++)
            {
                constructorArgs[i] = services.GetRequiredService(constructorParams[i].ParameterType);
            }

            return (TOperationTransformer)constructorToCall.Invoke(constructorArgs);
        }
    }
}
