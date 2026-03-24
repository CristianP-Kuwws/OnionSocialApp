using LinkUpApp.Core.Domain.Entities.Battleship;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUpApp.Infrastructure.Persistence.EntityConfigurations.Battleship
{
    public class BattleshipGameEntityConfiguration : IEntityTypeConfiguration<BattleshipGame>
    {
        public void Configure(EntityTypeBuilder<BattleshipGame> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("BattleshipGames");

            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.CreatorSetupStatus).IsRequired();
            builder.Property(x => x.OpponentSetupStatus).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.LastActivityAt);

            // FKs a Identity_Users
            builder.Property(x => x.CreatorUserId).IsRequired().HasMaxLength(255);
            builder.Property(x => x.OpponentUserId).IsRequired().HasMaxLength(255);
            builder.Property(x => x.CurrentTurnUserId).HasMaxLength(255);
            builder.Property(x => x.WinnerId).HasMaxLength(255);

            builder.HasIndex(x => x.CreatorUserId);
            builder.HasIndex(x => x.OpponentUserId);

            // Relaciones
            builder.HasMany(x => x.Ships)
                .WithOne(s => s.Game)
                .HasForeignKey(s => s.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Attacks)
                .WithOne(a => a.Game)
                .HasForeignKey(a => a.GameId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
