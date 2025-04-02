using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using MovieReservation.OpenApi.Transformers;
using Scalar.AspNetCore;

namespace MovieReservation.Startup
{
    public static class OpenApiStartupExtensions
    {
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
                        key: JwtBearerDefaults.AuthenticationScheme,
                        value: new OpenApiSecurityScheme
                        {
                            Description = "Jwt bearer token using \"Authorization\" header",
                            Scheme = "bearer",
                            BearerFormat = "JWT",
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.Http,
                            Name = "Authorization"
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

        public static IEndpointConventionBuilder MapOpenApiClient(this IEndpointRouteBuilder app)
        {
            var openApiClientGroup = app.MapGroup("/openapi");

            openApiClientGroup.MapOpenApi("/{documentName}.json");
            openApiClientGroup.MapScalarApiReference(options =>
            {
                options.WithTitle("Movie Reservation API")
                    .WithTheme(ScalarTheme.BluePlanet)
                    .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch)
                    .WithClientButton(false)
                    .WithForceThemeMode(ThemeMode.Dark)
                    .WithDarkModeToggle(false);
            });

            return openApiClientGroup;
        }
    }
}
