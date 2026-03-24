using LinkUpApp.Core.Domain.Entities.Battleship;
using LinkUpApp.Core.Domain.Entities.Social;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LinkUpApp.Infrastructure.Identity.Entities
{
    public class ApplicationUser : IdentityUser
    {
        // Custom properties
        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(60)]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es requerido.")]
        [StringLength(60)]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "El telefono es requerido.")]
        [StringLength(20)]
        [Phone(ErrorMessage = "El formato del telefono no es valido.")]
        public required string Phone { get; set; }

        [StringLength(300)]
        public string? ProfilePicturePath { get; set; }

        public bool IsActive { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties (usan entidades de Domain)
        // un usuario puede crear muchas publicaciones
        public ICollection<Post> Posts { get; set; } = new List<Post>();

        // un usuario puede hacer muchos comentarios
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        // un usuario puede tener muchos amigos (self-referencing)
        public ICollection<Friendship> FriendshipsInitiated { get; set; } = new List<Friendship>();
        public ICollection<Friendship> FriendshipsReceived { get; set; } = new List<Friendship>();

        // un usuario puede enviar muchas solicitudes de amistad
        public ICollection<FriendRequest> SentFriendRequests { get; set; } = new List<FriendRequest>();

        // un usuario puede recibir muchas solicitudes de amistad
        public ICollection<FriendRequest> ReceivedFriendRequests { get; set; } = new List<FriendRequest>();

        // un usuario puede reaccionar a muchos posts
        public ICollection<PostReaction> PostReactions { get; set; } = new List<PostReaction>();

        // un usuario puede crear muchas partidas de battleship
        public ICollection<BattleshipGame> GamesCreated { get; set; } = new List<BattleshipGame>();

        // un usuario puede ser invitado a muchas partidas
        public ICollection<BattleshipGame> GamesParticipated { get; set; } = new List<BattleshipGame>();

        // un usuario posiciona barcos en partidas
        public ICollection<Ship> Ships { get; set; } = new List<Ship>();

        // un usuario realiza ataques en partidas
        public ICollection<Attack> Attacks { get; set; } = new List<Attack>();
    }
}
