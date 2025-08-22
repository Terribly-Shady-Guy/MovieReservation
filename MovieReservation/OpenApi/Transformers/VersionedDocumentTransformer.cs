using Microsoft.AspNetCore.OpenApi;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Asp.Versioning.ApiExplorer;

namespace MovieReservation.OpenApi.Transformers
{
    public class VersionedDocumentTransformer : IOpenApiDocumentTransformer
    {
        private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;

        public VersionedDocumentTransformer(IApiVersionDescriptionProvider descriptionProvider)
        {
            _apiVersionDescriptionProvider = descriptionProvider;
        }

        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
           ApiVersionDescription description = _apiVersionDescriptionProvider.ApiVersionDescriptions
                .Single(d => d.GroupName == context.DocumentName);

            document.Info.Version = $"v{description.ApiVersion}";
            document.Info.Title = "Movie Reservation API";
            document.Info.Description = "An API for customers to view and reserve movies. Admin users can manage showings and view reservation reports.";

            if (description.IsDeprecated)
            {
                document.Info.Title += " (DEPRECATED)";
                document.Info.Description += $" \n> [!warning] This version of the API is deprecated and {(description.SunsetPolicy is not null ? $"will be removed on **{description.SunsetPolicy.Date}**" : "may be removed in the future")}.";
            }
            
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes.Add(
                key: "JWT Bearer",
                value: new OpenApiSecurityScheme
                {
                    Description = $"Jwt bearer token using ```{HeaderNames.Authorization}``` header",
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Name = HeaderNames.Authorization
                });
            
            return Task.CompletedTask;
        }
    }
}
