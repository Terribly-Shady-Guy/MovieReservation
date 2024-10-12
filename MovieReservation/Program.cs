using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieReservation.Models;
using MovieReservation.Services;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton((provider) =>
{
    var path = Path.Combine(Environment.CurrentDirectory, "..", "Rsa");
    var rsa = RSA.Create();

    if (!Directory.Exists(path))
    {
        Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(path, "key.xml"), rsa.ToXmlString(true));
    }
    else
    {
        rsa.FromXmlString(File.ReadAllText(Path.Combine(path, "key.xml")));
    }

    return new RsaSecurityKey(rsa);
});

builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<RsaSecurityKey>((options, signingKey) =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
            IssuerSigningKey = signingKey,
            ValidAudience = builder.Configuration.GetSection("Jwt").GetValue<string>("Audience"),
            ValidIssuer = builder.Configuration.GetSection("Jwt").GetValue<string>("Issuer")
        };
    });

builder.Services.AddAuthentication(configureOptions =>
{
    configureOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    configureOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer();

builder.Services.AddDbContext<MovieReservationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("default"));
});

builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<AuthenticationTokenManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
