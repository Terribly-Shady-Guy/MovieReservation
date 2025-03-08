using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using DbInfrastructure.Models;
using MovieReservation.Services;
using DbInfrastructure;
using Scalar.AspNetCore;
using Microsoft.OpenApi.Models;
using MovieReservation.OpenApi.Transformers;
using MovieReservation.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(options =>
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
        document.Components.SecuritySchemes.Add(JwtBearerDefaults.AuthenticationScheme,
            new OpenApiSecurityScheme
            {
                Description = "Jwt bearer token using Authorization header",
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Name = "Authorization"
            });

        return Task.CompletedTask;
    });

    // This is a temporary workaround until the new description property is added to ProducesResponseType.
    options.AddOperationTransformer((operation, context, cancellationToken) =>
    {
        var responseTypes = context.Description.ActionDescriptor.EndpointMetadata
                .OfType<ProducesResponseTypeWithDescriptionAttribute>();

        foreach (var responseType in responseTypes)
        {
            bool isStatusCodeInResponses = operation.Responses.TryGetValue(
                key: responseType.StatusCode.ToString(),
                value: out OpenApiResponse? response);

            if (responseType.Description is not null && isStatusCodeInResponses && response is not null)
            {
                response.Description = responseType.Description;
            }
        }

        return Task.CompletedTask;
    });

    options.AddOperationTransformer<JwtBearerSecurityRequirementTransformer>();
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
            ValidIssuer = jwtConfig.GetValue<string>("Issuer"),
            ClockSkew = TimeSpan.FromSeconds(15)
        };
    });

builder.Services.AddAuthentication(configureOptions =>
{
    configureOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    configureOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer();

builder.Services.AddIdentityCore<AppUser>(options =>
{
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;

    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddRoles<IdentityRole>()
    .AddSignInManager()
    .AddEntityFrameworkStores<MovieReservationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbInfrastructure(builder.Configuration.GetConnectionString("default"));

builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddTransient<AuthenticationService>();
builder.Services.AddTransient<IAuthenticationTokenProvider, AuthenticationTokenProvider>();
builder.Services.AddTransient<UserService>();

builder.Services.AddTransient<MovieService>();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddTransient<IFileHandler, LocalFileHandler>();
}

builder.Services.AddTransient<LocationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Movie Reservation API")
            .WithTheme(ScalarTheme.BluePlanet)
            .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch)
            .WithClientButton(false)
            .WithForceThemeMode(ThemeMode.Dark)
            .WithDarkModeToggle(false);
    });
    
    string imagesDirectoryPath = Path.Combine(app.Environment.ContentRootPath, "..", "Images");
    imagesDirectoryPath = Path.GetFullPath(imagesDirectoryPath);

    if (!Directory.Exists(imagesDirectoryPath))
    {
        Directory.CreateDirectory(imagesDirectoryPath);
    }

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(imagesDirectoryPath),
        RequestPath = "/Images"
    });
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
