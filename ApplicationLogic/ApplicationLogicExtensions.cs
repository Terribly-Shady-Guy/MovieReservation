using ApplicationLogic.Interfaces;
using ApplicationLogic.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApplicationLogic
{
    public static class ApplicationLogicExtensions
    {
        /// <summary>
        /// Adds services for core movie reservation logic.
        /// </summary>
        /// <param name="services">The service collection instance for the application.</param>
        /// <param name="config">The application configuration object.</param>
        /// <param name="environment">The application environment config.</param>
        /// <returns></returns>
        public static IServiceCollection AddApplicationLogicServices(this IServiceCollection services, IConfiguration config, IWebHostEnvironment environment)
        {
            services.AddTransient<AuthenticationService>();
            services.AddTransient<IAuthenticationTokenProvider, AuthenticationTokenProvider>();
            services.AddTransient<UserService>();
            services.AddSingleton<IRsaKeyHandler, LocalRsaKeyHandler>();
            services.AddTransient<LoginManager>();

            services.AddTransient<MovieService>();
            
            if (environment.IsDevelopment())
            {
                services.AddTransient<IFileHandler, LocalFileHandler>();
            }

            services.AddTransient<LocationService>();

            return services;
        }
    }
}
