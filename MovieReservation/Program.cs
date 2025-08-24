using DbInfrastructure;
using MovieReservation.Startup;
using ApplicationLogic;
using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});

builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApiDocuments();
}

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

builder.Services.AddIdentityJwtAuthentication(builder.Configuration.GetRequiredSection("Jwt"));

builder.Services.AddApplicationLogicServices(builder.Configuration, builder.Environment);
builder.Services.AddDbInfrastructure(builder.Configuration.GetConnectionString("default"));

builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks()
   .AddCheck<SqlServerHealthCheck>("dbconnection", null, ["db", "sql"]);

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApiReference();
}

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
