using LinkUpApp.Core.Domain.Common.Enum.Social;
using System.ComponentModel.DataAnnotations;

namespace LinkUpApp.Core.Domain.Entities.Social
{
    public class PostReaction
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El post es requerido.")]
        public required int PostId { get; set; }

        [Required(ErrorMessage = "El usuario es requerido.")]
        public required string UserId { get; set; }

        [Required(ErrorMessage = "El tipo de reaccion es requerido.")]
        public ReactionType Type { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Post Post { get; set; } = null!;

    }
}
