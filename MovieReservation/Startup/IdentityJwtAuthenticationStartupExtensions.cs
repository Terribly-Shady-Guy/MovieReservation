using DbInfrastructure.Models;
using DbInfrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using ApplicationLogic.Services;
using ApplicationLogic.Interfaces;
using ApplicationLogic;

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
            services.AddOptions<JwtOptions>()
                .Bind(jwtConfig)
                .Validate(options => options.LifetimeMinutes > 0, "lifetime minutes must be greater than 0.");

            services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .Configure<IRsaKeyHandler, IOptions<JwtOptions>>((options, keyHandler, config) =>
                {
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
                        ValidAudience = config.Value.Audience,
                        ValidIssuer = config.Value.Issuer,
                        ClockSkew = TimeSpan.FromSeconds(15)
                    };
                });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

            services.AddAuthorizationBuilder()
                .AddPolicy(RegisteredAuthorizationPolicyNames.IsAdmin, policy => policy.RequireRole(Roles.Admin, Roles.SuperAdmin));
            
            services.AddIdentityCore<AppUser>(options =>
            {
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;

                options.User.RequireUniqueEmail = true;

                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
            })
                .AddRoles<IdentityRole>()
                .AddSignInManager()
                .AddEntityFrameworkStores<MovieReservationDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }

    /// <summary>
    /// A collection of constants for registered authorization policies.
    /// </summary>
    public static class RegisteredAuthorizationPolicyNames
    {
        /// <summary>
        /// Represents the policy that requires the user to be an admin role.
        /// </summary>
        public const string IsAdmin = "IsAdmin";
    }
}
