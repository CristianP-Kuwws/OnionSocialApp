using LinkUpApp.Core.Domain.Interfaces.Base;
using LinkUpApp.Core.Domain.Interfaces.Battleship;
using LinkUpApp.Core.Domain.Interfaces.Social;
using LinkUpApp.Infrastructure.Persistence.Contexts;
using LinkUpApp.Infrastructure.Persistence.Repositories.Base;
using LinkUpApp.Infrastructure.Persistence.Repositories.Battleship;
using LinkUpApp.Infrastructure.Persistence.Repositories.Social;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkUpApp.Core.Persistence
{
    public static class ServicesRegistration
    {
        //Extension method - decorator pattern
        public static void AddPersistenceLayerIoc(this IServiceCollection services, IConfiguration config)
        {
            #region Contexts
            if (config.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<LinkUpAppContext>(options =>
                {
                    options.UseInMemoryDatabase("LinkUpInMemoryDb");
                });
            }
            else
            {
                var connectionString = config.GetConnectionString("DefaultConnection");

                services.AddDbContext<LinkUpAppContext>(
                    (serviceProvider, options) =>
                    {
                        options.EnableSensitiveDataLogging();
                        options.UseMySql(
                             connectionString,
                             ServerVersion.AutoDetect(connectionString),
                             mySqlOptions => mySqlOptions.MigrationsAssembly(typeof(LinkUpAppContext).Assembly.FullName)
                        );
                    },
                    contextLifetime: ServiceLifetime.Scoped,
                    optionsLifetime: ServiceLifetime.Scoped
                );
            }

            #endregion

            #region Repositories IOC

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();
            services.AddScoped<IPostReactionRepository, PostReactionRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IAttackRepository, AttackRepository>();
            services.AddScoped<IBattleshipGameRepository, BattleshipGameRepository>();
            services.AddScoped<IShipRepository, ShipRepository>();

            #endregion
        }

    }
 
}
