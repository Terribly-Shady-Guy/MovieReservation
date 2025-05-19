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
        private readonly Type _transformerType = typeof(TOperationTransformer);

        public string TransformerTypeName => _transformerType.FullName ?? _transformerType.Name;

        /// <summary>
        /// Creates an <see cref="IOpenApiOperationTransformer"/> object with dependency injection support.
        /// </summary>
        /// <param name="services"></param>
        /// <returns>An instance of the provided type.</returns>
        /// <exception cref="InvalidOperationException"/>
        public IOpenApiOperationTransformer CreateTransformer(IServiceProvider services)
        {
            ConstructorInfo[] constructors = _transformerType.GetConstructors();

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
            
            if (constructorToCall is null)
            {
                throw new InvalidOperationException($"The provided type \"{_transformerType.Name}\" does not contain a public constructor.");
            }

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
