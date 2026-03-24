using LinkUpApp.Core.Application.Interfaces.User;
using LinkUpApp.Infrastructure.Identity.Contexts;
using LinkUpApp.Infrastructure.Identity.Entities;
using LinkUpApp.Infrastructure.Identity.Seeds;
using LinkUpApp.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkUpApp.Infrastructure.Identity
{
    public static class ServicesRegistration
    {
        public static void AddIdentityInfrastructureLayerIoc(this IServiceCollection services, IConfiguration config)
        {
            // Configurar DbContext
            ConfigureDatabase(services, config);

            // Configurar opciones de Identity
            services.Configure<IdentityOptions>(opt =>
            {
                // Password
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = true;
                opt.Password.RequireNonAlphanumeric = false; // False para probar mas facil
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;

                // Lockout
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opt.Lockout.MaxFailedAccessAttempts = 5;

                // User
                opt.User.RequireUniqueEmail = true;

                // SignIn
                opt.SignIn.RequireConfirmedEmail = false; // False por testeo 
            });

            // Configurar Identity 
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            // Lifespan de tokens
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromHours(12);
            });

            // Configurar cookies de autenticacion
            services.ConfigureApplicationCookie(opt =>
            {
                opt.LoginPath = "/Login/Index";
                opt.AccessDeniedPath = "/Login/AccessDenied";
                opt.ExpireTimeSpan = TimeSpan.FromHours(3);
                opt.SlidingExpiration = true; // Renueva cookie si usuario esta activo
            });

            #region Services
            services.AddScoped<IAccountServicesForWebApp, AccountServicesForWebApp>();
            #endregion
        }

        public static async Task RunIdentitySeedAsync(this IServiceProvider service) 
        {
            using var scoped = service.CreateScope();
            var servicesProvider = scoped.ServiceProvider;

            var userManager = servicesProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = servicesProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await DefaultRoles.SeedAsync(roleManager);
            await DefaultUser.SeedAsync(userManager);
        }

        private static void ConfigureDatabase(IServiceCollection services, IConfiguration config)
        {
            if (config.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<IdentityContext>(options =>
                {
                    options.UseInMemoryDatabase("LinkUpIdentityDb");
                });
            }
            else
            {
                var connectionString = config.GetConnectionString("DefaultConnection");

                services.AddDbContext<IdentityContext>(
                    (serviceProvider, options) =>
                    {
                        options.EnableSensitiveDataLogging();
                        options.UseMySql(
                            connectionString,
                            ServerVersion.AutoDetect(connectionString),
                            mySqlOptions => mySqlOptions.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)
                        );
                    },
                    contextLifetime: ServiceLifetime.Scoped,
                    optionsLifetime: ServiceLifetime.Scoped
                );
            }
        }
    }
}
