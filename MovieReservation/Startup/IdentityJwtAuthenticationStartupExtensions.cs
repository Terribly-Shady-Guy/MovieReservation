using DbInfrastructure.Models;
using DbInfrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MovieReservation.Services;

namespace MovieReservation.Startup
{
    public static class IdentityJwtAuthenticationStartupExtensions
    {
        /// <summary>
        /// Adds the services for enabling Jwt authentication with identity framework.
        /// </summary>
        /// <param name="services">The application service collection instance.</param>
        /// <param name="jwtConfig">The configuration section containing jwt configs.</param>
        /// <returns>The same service collection instance from parameter.</returns>
        public static IServiceCollection AddIdentityJwtAuthentication(this IServiceCollection services, IConfigurationSection jwtConfig)
        {
            services.AddSingleton<IRsaKeyHandler, LocalRsaKeyHandler>();

            services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
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

            services.AddAuthentication(configureOptions =>
            {
                configureOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                configureOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer();

            services.AddIdentityCore<AppUser>(options =>
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

            services.Configure<JwtOptions>(jwtConfig);
            services.AddTransient<AuthenticationService>();
            services.AddTransient<IAuthenticationTokenProvider, AuthenticationTokenProvider>();
            services.AddTransient<UserService>();

            return services;
        }
    }
}
