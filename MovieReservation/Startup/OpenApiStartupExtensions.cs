using ApplicationLogic.Interfaces;
using ApplicationLogic.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MovieReservation.OpenApi.Transformers;
using Scalar.AspNetCore;
using System.Security.Claims;

namespace MovieReservation.Startup
{
    public static class OpenApiStartupExtensions
    {
        private const string BearerSchemeKey = "JWT Bearer";
        /// <summary>
        /// Adds the OpenAPI services for generating documents.
        /// </summary>
        /// <param name="services">The application service collection instance.</param>
        /// <returns>The same service collection instance from paramater.</returns>
        public static IServiceCollection AddOpenApiServices(this IServiceCollection services)
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new OpenApiInfo
                    {
                        Version = "V1",
                        Title = "Movie Reservation API",
                        Description = "An API for customers to view and reserve movies. Admin users can manage showings and view reservation reports."
                    };

                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes.Add(
                        key: BearerSchemeKey,
                        value: new OpenApiSecurityScheme
                        {
                            Description = $"Jwt bearer token using \"{HeaderNames.Authorization}\" header",
                            Scheme = "bearer",
                            BearerFormat = "JWT",
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.Http,
                            Name = HeaderNames.Authorization
                        });
                    
                    return Task.CompletedTask;
                });

                // This is a temporary workaround until the new description property is added to ProducesResponseType.
                options.AddOperationTransformer<EndpointResponseDescriptionTransformer>();
                options.AddOperationTransformer<EndpointOperationTransformer>();
                options.AddOperationTransformer<JwtBearerSecurityRequirementTransformer>();
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
                    .AddHttpAuthentication(BearerSchemeKey, scheme => scheme.Token = token);
            });
        }
    }
}
