using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace MovieReservation.OpenApiTransformers
{
    public class JwtBearerSecurityRequirementTransformer : IOpenApiOperationTransformer
    {
        private readonly IAuthenticationSchemeProvider _schemeProvider;

        public JwtBearerSecurityRequirementTransformer(IAuthenticationSchemeProvider schemeProvider)
        {
            _schemeProvider = schemeProvider;
        }

        public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var scheme = await _schemeProvider.GetSchemeAsync("Bearer");
            if (scheme is null) return;

            bool hasAuthData = context.Description.ActionDescriptor.EndpointMetadata
                 .OfType<IAuthorizeData>()
                 .Any(authData => authData.AuthenticationSchemes is null || authData.AuthenticationSchemes.Contains(scheme.Name));

            if (!hasAuthData) return;

            operation.Responses.Add(StatusCodes.Status403Forbidden.ToString(), new OpenApiResponse
            {
                Description = "User does not have required role or token is invalid."
            });

            operation.Responses.TryAdd(StatusCodes.Status401Unauthorized.ToString(), new OpenApiResponse
            {
                Description = "The access token has not been provided in \"Authorization\" header."
            });

            var requirementSecurityScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = scheme.Name,
                    Type = ReferenceType.SecurityScheme
                }
            };

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [requirementSecurityScheme] = Array.Empty<string>()
            });
        }
    }
}
