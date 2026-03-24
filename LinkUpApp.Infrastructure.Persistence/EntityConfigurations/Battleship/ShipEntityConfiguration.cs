using LinkUpApp.Core.Domain.Entities.Battleship;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUpApp.Infrastructure.Persistence.EntityConfigurations.Battleship
{
    public class ShipEntityConfiguration : IEntityTypeConfiguration<Ship>
    {
        public void Configure(EntityTypeBuilder<Ship> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Ships");

            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.StartRow).IsRequired();
            builder.Property(x => x.StartColumn).IsRequired();
            builder.Property(x => x.Orientation).IsRequired();
            builder.Property(x => x.IsSunk).IsRequired();
            builder.Property(x => x.PositionedAt).IsRequired();

            builder.HasIndex(x => new { x.GameId, x.UserId });

            // FK a Identity_Users
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(255);
            builder.HasIndex(x => x.UserId);

            // Relaciones
            builder.HasOne(x => x.Game)
                .WithMany(g => g.Ships)
                .HasForeignKey(x => x.GameId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
