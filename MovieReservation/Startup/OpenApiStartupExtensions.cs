using ApplicationLogic.Interfaces;
using ApplicationLogic.Services;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MovieReservation.OpenApi.Transformers;
using Scalar.AspNetCore;
using System.Security.Claims;

namespace MovieReservation.Startup
{
    public static class OpenApiStartupExtensions
    {
        /// <summary>
        /// Registers the OpenAPI documents for each API version.
        /// </summary>
        /// <param name="services">The application service collection instance.</param>
        /// <returns>The same service collection instance.</returns>
        public static IServiceCollection AddOpenApiDocuments(this IServiceCollection services)
        {
            services.AddOpenApi("v1", options =>
            {
                options.AddDocumentTransformer<VersionedDocumentTransformer>();

                // This is a temporary workaround until the new description property is added to ProducesResponseType.
                options.AddOperationTransformer<EndpointResponseDescriptionTransformer>();

                options.AddOperationTransformer<EndpointOperationTransformer>();
                options.AddOperationTransformer<JwtBearerSecurityRequirementTransformer>();
                options.AddOperationTransformer<OperationDeprecatedTransformer>();
            });

            return services;
        }

        /// <summary>
        /// Maps endpoints for OpenAPI document generation and API reference UI.
        /// </summary>
        /// <param name="routeBuilder">A route builder from either <see cref="WebApplication"/> or a <see cref="RouteGroupBuilder"/>.</param>
        public static void MapOpenApiReference(this IEndpointRouteBuilder routeBuilder)
        {
            var rsaKeyHandler = routeBuilder.ServiceProvider.GetRequiredService<IRsaKeyHandler>();
            var options = routeBuilder.ServiceProvider.GetRequiredService<IOptions<JwtOptions>>();
            var descriptionProvider = routeBuilder.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

            RsaSecurityKey securityKey = rsaKeyHandler.LoadPrivateAsync()
                .GetAwaiter()
                .GetResult();

            var descriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256),
                Expires = DateTime.UtcNow.AddDays(7),
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Role, "Admin")
                ]),
                Audience = options.Value.Audience,
                Issuer = options.Value.Issuer,
            };

            var tokenHandler = new JsonWebTokenHandler();
            string token = tokenHandler.CreateToken(descriptor);

            const string BearerSchemeKey = "JWT Bearer";

            RouteGroupBuilder openApiReferenceGroup = routeBuilder.MapGroup("/api-reference")
                .ExcludeFromDescription();

            openApiReferenceGroup.MapOpenApi();
            openApiReferenceGroup.MapScalarApiReference(options =>
            {
                options.WithTitle("Movie Reservation API")
                    .WithTheme(ScalarTheme.Saturn)
                    .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch)
                    .WithClientButton(false)
                    .WithOpenApiRoutePattern("/api-reference/openapi/{documentName}.json")
                    .WithSearchHotKey("s")
                    .AddPreferredSecuritySchemes(BearerSchemeKey)
                    .WithPersistentAuthentication()
                    .AddHttpAuthentication(BearerSchemeKey, scheme => scheme.Token = token)
                    .AddDocuments(descriptionProvider.ApiVersionDescriptions.Select(d => d.GroupName));
            });
        }
    }
}
