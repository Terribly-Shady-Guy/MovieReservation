using Microsoft.AspNetCore.OpenApi;
using System.Reflection;

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

        public IOpenApiOperationTransformer? CreateTransformer(IServiceProvider services)
        {
            ConstructorInfo[] constructors = TransformerType.GetConstructors();

            ConstructorInfo? constructorToCall = null;
            ParameterInfo[] constructorParams = [];

            foreach (ConstructorInfo constructor in constructors)
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
                Type paramType = constructorParams[i].ParameterType;
                constructorArgs[i] = services.GetRequiredService(paramType);
            }

            return (TOperationTransformer)constructorToCall.Invoke(constructorArgs);
        }
    }
}
