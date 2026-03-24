using LinkUpApp.Core.Domain.Entities.Battleship;
using LinkUpApp.Core.Domain.Entities.Social;
using LinkUpApp.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LinkUpApp.Infrastructure.Persistence.Contexts
{
    public class LinkUpAppContext : DbContext 
    {
        public LinkUpAppContext(DbContextOptions<LinkUpAppContext> options) : base(options) { }

        // Social
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostReaction> PostReactions { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }

        // Battleship
        public DbSet<BattleshipGame> BattleshipGames { get; set; }
        public DbSet<Ship> Ships { get; set; }
        public DbSet<Attack> Attacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ignorar ApplicationUser para que no se cree la tabla aqui
            // La tabla se crea en IdentityContext
            modelBuilder.Ignore<ApplicationUser>();

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
    }
}
