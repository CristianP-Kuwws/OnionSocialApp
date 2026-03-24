using System.ComponentModel.DataAnnotations;

namespace LinkUpApp.Core.Domain.Entities.Social
{
    public class Friendship
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El primer usuario es requerido.")]
        public required string User1Id { get; set; }

        [Required(ErrorMessage = "El segundo usuario es requerido.")]
        public required string User2Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
