using LinkUpApp.Core.Domain.Entities.Battleship;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUpApp.Infrastructure.Persistence.EntityConfigurations.Battleship
{
    public class AttackEntityConfiguration : IEntityTypeConfiguration<Attack>
    {
        public void Configure(EntityTypeBuilder<Attack> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Attacks");

            builder.Property(x => x.TargetRow).IsRequired();
            builder.Property(x => x.TargetColumn).IsRequired();
            builder.Property(x => x.Result).IsRequired();
            builder.Property(x => x.TurnNumber).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            builder.HasIndex(x => new { x.GameId, x.TargetRow, x.TargetColumn, x.AttackingUserId })
                .IsUnique();

            builder.HasIndex(x => new { x.GameId, x.TurnNumber });

            // FK a Identity_Users
            builder.Property(x => x.AttackingUserId).IsRequired().HasMaxLength(255);
            builder.HasIndex(x => x.AttackingUserId);

            // Relaciones
            builder.HasOne(x => x.Game)
                .WithMany(g => g.Attacks)
                .HasForeignKey(x => x.GameId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
