using LinkUpApp.Core.Application.Helpers;
using LinkUpApp.Core.Application.Interfaces.Battleship;
using LinkUpApp.Core.Application.Interfaces.Helpers;
using LinkUpApp.Core.Application.Interfaces.Social;
using LinkUpApp.Core.Application.Services.Battleship;
using LinkUpApp.Core.Application.Services.Social;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LinkUpApp.Core.Application
{
    public static class ServicesRegistration
    {
        public static void AddApplicationLayerIoc(this IServiceCollection services)
        {

            #region Mappings
            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
            #endregion

            #region ServicesIOC
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IFriendRequestService, FriendRequestService>();
            services.AddScoped<IFriendshipService, FriendshipService>();
            services.AddScoped<IPostReactionService, PostReactionService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IBattleshipService, BattleshipService>();

            //helpers
            services.AddScoped<IBoardBuilder, BoardBuilder>();

            #endregion
        }
    }
}
