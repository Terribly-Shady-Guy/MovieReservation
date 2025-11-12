using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi;

namespace MovieReservation.OpenApi.Transformers
{
    public sealed class JwtBearerSecurityRequirementTransformer : IOpenApiOperationTransformer
    {
        private readonly IAuthenticationSchemeProvider _schemeProvider;

        public JwtBearerSecurityRequirementTransformer(IAuthenticationSchemeProvider schemeProvider)
        {
            _schemeProvider = schemeProvider;
        }

        public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            AuthenticationScheme? jwtBearerScheme = await _schemeProvider.GetSchemeAsync("Bearer");
            if (jwtBearerScheme is null)
            {
                return;
            }

            bool hasBearerAuthData = context.Description.ActionDescriptor.EndpointMetadata
                 .OfType<IAuthorizeData>()
                 .Any(authData => authData.AuthenticationSchemes is null || authData.AuthenticationSchemes.Contains(jwtBearerScheme.Name));

            if (!hasBearerAuthData)
            {
                return;
            }

            operation.Responses?.Add(StatusCodes.Status403Forbidden.ToString(), new OpenApiResponse
            {
                Description = "User does not have required role or token is invalid."
            });

            operation.Responses?.TryAdd(StatusCodes.Status401Unauthorized.ToString(), new OpenApiResponse
            {
                Description = $"The access token has not been provided in ```{HeaderNames.Authorization}``` header."
            });

            var requirementSecurityScheme = new OpenApiSecuritySchemeReference(jwtBearerScheme.Name, context.Document);

            operation.Security?.Add(new OpenApiSecurityRequirement
            {
                [requirementSecurityScheme] = []
            });
        }
    }
}
