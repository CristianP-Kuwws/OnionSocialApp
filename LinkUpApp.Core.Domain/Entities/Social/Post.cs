using LinkUpApp.Core.Domain.Common.Enum.Social;
using System.ComponentModel.DataAnnotations;

namespace LinkUpApp.Core.Domain.Entities.Social
{
    public class Post
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El usuario es requerido.")]
        public required string UserId { get; set; }

        [Required(ErrorMessage = "El contenido es requerido.")]
        [StringLength(5000)]
        public required string Content { get; set; }

        [Required(ErrorMessage = "El tipo de publicacion es requerido.")]
        public PostType Type { get; set; }

        [StringLength(500)]
        public string? MediaPath { get; set; } // ruta de imagen si Type = Image

        [StringLength(500)]
        [Url(ErrorMessage = "La URL de YouTube no es valida.")]
        public string? YouTubeUrl { get; set; } // url de video si Type = Video

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<PostReaction> Reactions { get; set; } = new List<PostReaction>();
    }
}
