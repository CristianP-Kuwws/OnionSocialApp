using LinkUpApp.Core.Domain.Entities.Social;
using LinkUpApp.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUpApp.Infrastructure.Persistence.EntityConfigurations.Social
{
    public class CommentEntityConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Comments");

            builder.Property(x => x.Content).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.CreatedAt).IsRequired();

            // FK a Identity_Users
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(255);
            builder.HasIndex(x => x.UserId);

            // Relaciones
            builder.HasOne(x => x.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(x => x.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
