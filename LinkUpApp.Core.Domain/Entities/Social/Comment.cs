using System.ComponentModel.DataAnnotations;

namespace LinkUpApp.Core.Domain.Entities.Social
{
    public class Comment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El post es requerido.")]
        public required int PostId { get; set; }

        [Required(ErrorMessage = "El usuario es requerido.")]
        public required string UserId { get; set; }
        public int? ParentCommentId { get; set; }

        [Required(ErrorMessage = "El contenido es requerido.")]
        [StringLength(2000)]
        public required string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public Post Post { get; set; } = null!;
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}
