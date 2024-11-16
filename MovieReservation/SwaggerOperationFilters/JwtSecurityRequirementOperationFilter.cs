using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MovieReservation.SwaggerOperationFilters
{
    public class JwtSecurityRequirementOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            const string SecurityDefinitionId = "Bearer";

            var methodAuthAttributes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>();

            var endpointAuthAttributes = context.MethodInfo.DeclaringType
                ?.GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Concat(methodAuthAttributes) ?? methodAuthAttributes;

            if (!endpointAuthAttributes.Any())
            {
                return;
            }
            
            operation.Responses.Add(StatusCodes.Status403Forbidden.ToString(), new OpenApiResponse
            {
                Description = "User does not have required role or token is invalid."
            });

            operation.Responses.TryAdd(StatusCodes.Status401Unauthorized.ToString(), new OpenApiResponse
            {
                Description = "The access token has not been provided in \"Authorization header\"."
            });

            var requirementScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = SecurityDefinitionId,
                    Type = ReferenceType.SecurityScheme
                }
            };

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                { requirementScheme, Array.Empty<string>() }
            });
        }
    }
}
