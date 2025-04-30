using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MovieReservation.OpenApi.Transformers;
using Scalar.AspNetCore;

namespace MovieReservation.Startup
{
    public static class OpenApiStartupExtensions
    {
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
                        key: "JWT Bearer",
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
        /// <returns>The builder for the /apireference route group.</returns>
        public static IEndpointConventionBuilder MapOpenApiReference(this IEndpointRouteBuilder routeBuilder)
        {
            const string RouteGroupPath = "/apireference";
            var openApiReferenceGroup = routeBuilder.MapGroup(RouteGroupPath)
                .ExcludeFromDescription();

            openApiReferenceGroup.MapOpenApi();
            openApiReferenceGroup.MapScalarApiReference(options =>
            {
                options.WithTitle("Movie Reservation API")
                    .WithTheme(ScalarTheme.Saturn)
                    .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch)
                    .WithClientButton(false)
                    .WithOpenApiRoutePattern($$"""{{RouteGroupPath}}/openapi/{documentName}.json""")
                    .WithSearchHotKey("s")
                    .WithPreferredScheme("JWT Bearer");
            });
            
            return openApiReferenceGroup;
        }
    }
}
