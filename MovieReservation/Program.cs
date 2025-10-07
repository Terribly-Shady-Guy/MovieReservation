using DbInfrastructure;
using MovieReservation.Startup;
using ApplicationLogic;
using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApiDocuments();
}

builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
    options.DefaultApiVersion = new ApiVersion(1, 0);
})
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.SubstituteApiVersionInUrl = true;
        options.GroupNameFormat = "'v'V";
    });

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy<string, AuthenticationRateLimiterPolicy>("Authentication");
});

builder.Services.AddIdentityJwtAuthentication(builder.Configuration.GetRequiredSection("Jwt"));

builder.Services.AddApplicationLogicServices(builder.Configuration, builder.Environment);
builder.Services.AddDbInfrastructure(builder.Configuration.GetConnectionString("default"));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<NotImplementedExceptionHandler>();

builder.Services.AddHealthChecks()
   .AddCheck<SqlServerHealthCheck>("dbconnection", null, ["db", "sql"]);

builder.Services.AddOutputCache();

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.UseOutputCache();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApiReference();
}

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();

// Added as I don't want to convert to program style main, but needed a type that is guaranteed to exist for tests.
public partial class Program;
