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
        public static IServiceCollection AddApplicationLogicServices(this IServiceCollection services, IConfiguration config, IWebHostEnvironment environment)
        {
            services.AddTransient<AuthenticationService>();
            services.AddTransient<IAuthenticationTokenProvider, AuthenticationTokenProvider>();
            services.AddTransient<UserService>();
            services.AddSingleton<IRsaKeyHandler, LocalRsaKeyHandler>();

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
