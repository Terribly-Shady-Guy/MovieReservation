using DbInfrastructure;
using MovieReservation.Startup;
using ApplicationLogic;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});

builder.Services.AddControllers();

builder.Services.AddOpenApiServices();

builder.Services.AddIdentityJwtAuthentication(builder.Configuration.GetRequiredSection("Jwt"));

builder.Services.AddApplicationLogicServices(builder.Configuration, builder.Environment);
builder.Services.AddDbInfrastructure(builder.Configuration.GetConnectionString("default"));

builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    string imagesDirectoryPath = Path.Combine(app.Environment.ContentRootPath, "..", "Images");
    imagesDirectoryPath = Path.GetFullPath(imagesDirectoryPath);

    if (!Directory.Exists(imagesDirectoryPath))
    {
        Directory.CreateDirectory(imagesDirectoryPath);
    }

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(imagesDirectoryPath),
        RequestPath = "/images"
    });

    app.MapOpenApiReference();
}

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
