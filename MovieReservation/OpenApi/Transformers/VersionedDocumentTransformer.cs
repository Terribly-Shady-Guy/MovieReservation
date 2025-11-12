using Microsoft.AspNetCore.OpenApi;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi;
using Asp.Versioning.ApiExplorer;

namespace MovieReservation.OpenApi.Transformers
{
    public class VersionedDocumentTransformer : IOpenApiDocumentTransformer
    {
        private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;
        private readonly ILogger<VersionedDocumentTransformer> _logger;

        public VersionedDocumentTransformer(IApiVersionDescriptionProvider descriptionProvider, ILogger<VersionedDocumentTransformer> logger)
        {
            _apiVersionDescriptionProvider = descriptionProvider;
            _logger = logger;
        }

        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
           ApiVersionDescription versionDescription = _apiVersionDescriptionProvider.ApiVersionDescriptions
                .Single(d => d.GroupName == context.DocumentName);

            _logger.LogInformation("Executing transformer for api group {GroupName}.", versionDescription.GroupName);

            document.Info.Version = $"v{versionDescription.ApiVersion}";
            document.Info.Title = "Movie Reservation API";
            document.Info.Description = "An API for customers to view and reserve movies. Admin users can manage showings and view reservation reports.";

            if (versionDescription.IsDeprecated)
            {
                document.Info.Description += $"\n> [!warning] This version of the API is deprecated and {(versionDescription.SunsetPolicy is not null ? $"will be removed on **{versionDescription.SunsetPolicy.Date}**" : "may be removed in the future")}.";
            }
            
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
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
