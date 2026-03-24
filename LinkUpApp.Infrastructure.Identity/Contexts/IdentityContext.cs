using LinkUpApp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LinkUpApp.Infrastructure.Identity.Contexts
{
    public class IdentityContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // MySQL no soporta esquemas, asi que en este caso 
            // use prefijos en nombres de tabla en lugar de schemas
            // O simplemente tablas separadas en la misma BD

            // Tablas de Identity con prefijo
            builder.Entity<ApplicationUser>().ToTable("Identity_Users");
            builder.Entity<IdentityRole>().ToTable("Identity_Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("Identity_UserRoles");
            builder.Entity<IdentityUserLogin<string>>().ToTable("Identity_UserLogins");
            builder.Entity<IdentityUserClaim<string>>().ToTable("Identity_UserClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("Identity_UserTokens");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("Identity_RoleClaims");

            // Configurar ApplicationUser
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(60);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(60);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.ProfilePicturePath).HasMaxLength(300);
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                // Se ignoran navigation properties en IdentityContext ya que
                // estas se manejaran en ApplicationDbContext
                entity.Ignore(e => e.Posts);
                entity.Ignore(e => e.Comments);
                entity.Ignore(e => e.FriendshipsInitiated);
                entity.Ignore(e => e.FriendshipsReceived);
                entity.Ignore(e => e.SentFriendRequests);
                entity.Ignore(e => e.ReceivedFriendRequests);
                entity.Ignore(e => e.PostReactions);
                entity.Ignore(e => e.GamesCreated);
                entity.Ignore(e => e.GamesParticipated);
                entity.Ignore(e => e.Ships);
                entity.Ignore(e => e.Attacks);
            });
        }
    }
}
