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
                .Union(methodAuthAttributes) ?? methodAuthAttributes;

            if (endpointAuthAttributes.Any())
            {
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
}
