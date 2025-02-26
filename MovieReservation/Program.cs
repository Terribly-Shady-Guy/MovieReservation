using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using DbInfrastructure.Models;
using MovieReservation.Services;
using MovieReservation.SwaggerOperationFilters;
using System.Reflection;
using DbInfrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});

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
            ValidIssuer = jwtConfig.GetValue<string>("Issuer"),
            ClockSkew = TimeSpan.FromSeconds(30)
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
    app.UseSwagger();
    app.UseSwaggerUI();
    
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
