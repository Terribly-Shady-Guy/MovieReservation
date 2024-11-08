using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MovieReservation.SwaggerOperationFilters
{
    public class JwtSecurityRequirementOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var methodScopes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Distinct();

            var combinedScopes = context.MethodInfo?.DeclaringType
                ?.GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Distinct()
                .Union(methodScopes) ?? methodScopes;


            if (combinedScopes.Any())
            {
                var requirementScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };

                operation.Security =
                [
                    new OpenApiSecurityRequirement
                    {
                         { requirementScheme, Array.Empty<string>() }
                    }
                ];
            }
        }
    }
}
