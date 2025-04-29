using Microsoft.AspNetCore.Mvc;

namespace MovieReservation.OpenApi
{
    ///<inheritdoc/>
    /// <remarks>This filter can also additionally contain a description for context. 
    /// This is meant to be temporary until Description is added to <see cref="ProducesResponseTypeAttribute" />.</remarks>
#if !NET9_0
       [Obsolete("Please use Description property in ProducesResponseType instead")]
#endif
    public class ProducesResponseTypeWithDescriptionAttribute : ProducesResponseTypeAttribute
    {
        public ProducesResponseTypeWithDescriptionAttribute(int statusCode) : base(statusCode)
        {
            
        }

        protected ProducesResponseTypeWithDescriptionAttribute(Type type, int statusCode) : base(type, statusCode)
        {

        }

        protected ProducesResponseTypeWithDescriptionAttribute(Type type, int statusCode, string contentType, params string[] additionalContentTypes)
            : base(type, statusCode, contentType, additionalContentTypes)
        {

        }

        /// <summary>
        /// Adds an optional description to the response in the OpenApi document.
        /// </summary>
        public string? Description { get; set; }
    }

    /// <inheritdoc/>
    /// <typeparam name="TResponseType">The type returned in the response</typeparam>
    public class ProducesResponseTypeWithDescriptionAttribute<TResponseType> : ProducesResponseTypeWithDescriptionAttribute
    {
        public ProducesResponseTypeWithDescriptionAttribute(int statusCode) : base(typeof(TResponseType), statusCode)
        {

        }

        public ProducesResponseTypeWithDescriptionAttribute(int statusCode, string contentType, params string[] additionalContentTypes)
            : base(typeof(TResponseType), statusCode, contentType, additionalContentTypes)
        {

        }
    }
}
