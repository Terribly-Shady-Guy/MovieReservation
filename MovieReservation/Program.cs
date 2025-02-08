using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MovieReservation;
using MovieReservation.Data.DbContexts;
using MovieReservation.Models;
using MovieReservation.Services;
using MovieReservation.SwaggerOperationFilters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "V1",
        Title = "Movie Reservation API",
        Description = "An API for customers to view and reserve movies. Admin users can manage showings and view reservation reports."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Jwt bearer token using Authorization header",
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Name = "Authorization"
    });

    options.OperationFilter<JwtSecurityRequirementOperationFilter>();

    string xmlCommentFilePath = Path.Combine(AppContext.BaseDirectory, Assembly.GetExecutingAssembly().GetName().Name + ".xml");

    options.IncludeXmlComments(xmlCommentFilePath);
});

builder.Services.AddSingleton<IRsaKeyHandler, LocalRsaKeyHandler>();

builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IRsaKeyHandler, IConfiguration>((options, keyHandler, config) =>
    {
        var jwtConfig = config.GetSection("Jwt");

        if (!keyHandler.KeyExists())
        {
            keyHandler.SaveKey();
        }

        RsaSecurityKey signingKey = keyHandler.LoadPublicAsync()
            .GetAwaiter()
            .GetResult();

        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
            IssuerSigningKey = signingKey,
            ValidAudience = jwtConfig.GetValue<string>("Audience"),
            ValidIssuer = jwtConfig.GetValue<string>("Issuer")
        };
    });

builder.Services.AddAuthentication(configureOptions =>
{
    configureOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    configureOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer();

builder.Services.AddIdentityCore<AppUser>(config =>
{
    config.Password.RequireNonAlphanumeric = true;
    config.Password.RequiredLength = 8;
    config.Password.RequireUppercase = true;
    config.Password.RequireLowercase = true;
    config.Password.RequireDigit = true;

    config.SignIn.RequireConfirmedEmail = true;
    config.SignIn.RequireConfirmedAccount = true;
})
    .AddRoles<IdentityRole>()
    .AddSignInManager()
    .AddEntityFrameworkStores<MovieReservationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<MovieReservationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("default"), config =>
    {
        config.EnableRetryOnFailure();
    });

    options.UseDataSeeding();
});

builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks();

builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<AuthenticationTokenManager>();

builder.Services.AddTransient<MovieService>();
builder.Services.AddTransient<IFileHandler, LocalFileHandler>();

builder.Services.AddTransient<LocationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
