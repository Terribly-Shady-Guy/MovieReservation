using DbInfrastructure;
using MovieReservation.Startup;
using ApplicationLogic;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ApplicationLogic.Interfaces;
using System.Diagnostics;

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

app.MapGet("/Images/{fileName}", Results<FileStreamHttpResult, ProblemHttpResult>(string fileName, [FromServices] IFileHandler handler) =>
{
    var contentTypes = new Dictionary<string, string>
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png"
    };
    
    string? ext = Path.GetExtension(fileName);

    if (ext is null || !contentTypes.TryGetValue(ext, out string? contentType))
    {
        return TypedResults.Problem(
            title: "File issue",
            detail: "This is not a supported file type.",
            statusCode: StatusCodes.Status400BadRequest);
    }

    var result = handler.GetFile(fileName);

    if (!result.Success || result.FileStream is null)
    {
        return TypedResults.Problem(
            title: "File retrieval error",
            detail: "This file does not exist.",
            statusCode: StatusCodes.Status404NotFound);
    }

    return TypedResults.File(result.FileStream, contentType, result.FileName);
})
    .RequireAuthorization()
    .WithName("Images");

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
