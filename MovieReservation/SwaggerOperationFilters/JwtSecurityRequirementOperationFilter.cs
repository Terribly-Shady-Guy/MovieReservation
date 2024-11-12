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

            var methodScopes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>();

            var combinedScopes = context.MethodInfo.DeclaringType
                ?.GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Union(methodScopes) ?? methodScopes;

            if (combinedScopes.Any())
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
