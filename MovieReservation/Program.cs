using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

    string docXmlFileName = Assembly.GetExecutingAssembly()
        .GetName()
        .Name + ".xml";

    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, docXmlFileName));
});

builder.Services.AddTransient<IRsaKeyHandler, LocalRsaKeyHandler>();

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

builder.Services.AddIdentityCore<AppUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<MovieReservationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<MovieReservationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("default"), config =>
    {
        config.EnableRetryOnFailure();
    });

    options.UseSeeding((context, _) =>
    {
        string id = Guid.NewGuid().ToString();

        var user = context.Set<AppUser>().Find(id);

        if (user is not null)
        {
            return;
        }

        var hasher = new PasswordHasher<AppUser>();

        AppUser seededAdmin = new AppUser
        {
            Id = id,
            UserName = "root",
            AccessFailedCount = 0,
            Email = "root@example.com",
            FirstName = "root",
            LastName = "root",
        };

        seededAdmin.PasswordHash = hasher.HashPassword(seededAdmin, "admin246810");

        context.Add(seededAdmin);
        context.SaveChanges();
    });

    options.UseAsyncSeeding(async(context, _, cancellationToken) =>
    {
        string id = Guid.NewGuid().ToString();

        var user = await context.Set<AppUser>().FindAsync(id);

        if (user is not null)
        {
            return;
        }

        var hasher = new PasswordHasher<AppUser>();

        AppUser seededAdmin = new AppUser
        {
            Id = id,
            UserName = "root",
            AccessFailedCount = 0,
            Email = "root@example.com",
            FirstName = "root",
            LastName = "root",
        };

        seededAdmin.PasswordHash = hasher.HashPassword(seededAdmin, "admin246810");

        await context.AddAsync(seededAdmin, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

    });
});

builder.Services.AddProblemDetails();

builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<AuthenticationTokenManager>();

builder.Services.AddTransient<MovieService>();
builder.Services.AddTransient<IFileHandler, LocalFileHandler>();

builder.Services.AddTransient<LocationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
