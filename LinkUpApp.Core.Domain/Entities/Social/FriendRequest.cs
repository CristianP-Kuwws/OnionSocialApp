using LinkUpApp.Core.Domain.Common.Enum.Social;
using System.ComponentModel.DataAnnotations;

namespace LinkUpApp.Core.Domain.Entities.Social
{
    public class FriendRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El remitente es requerido.")]
        public required string SenderId { get; set; }

        [Required(ErrorMessage = "El destinatario es requerido.")]
        public required string ReceiverId { get; set; }

        [Required(ErrorMessage = "El estado es requerido.")]
        public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RespondedAt { get; set; }

    }
}
