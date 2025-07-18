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
    app.MapOpenApiReference();
}

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
