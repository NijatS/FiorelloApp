using Fir.App.Context;
using Fir.App.Services.Implementations;
using Fir.App.Services.Interfaces;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fir.App.ServiceRegistrations
{
    public static class ServiceRegister
    {
        public static void Register(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<FirDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("Default"));
            });
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IMailService, MailService>();
            services.AddIdentity<AppUser, IdentityRole>()
                     .AddDefaultTokenProviders()
                            .AddEntityFrameworkStores<FirDbContext>();
            services.Configure<IdentityOptions>(options =>
            {
                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            });
        }
    }
}
