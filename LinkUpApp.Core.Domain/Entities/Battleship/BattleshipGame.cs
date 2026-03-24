using LinkUpApp.Core.Domain.Common.Enum.Battleship;
using System.ComponentModel.DataAnnotations;

namespace LinkUpApp.Core.Domain.Entities.Battleship
{
    public class BattleshipGame
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El creador es requerido.")]
        public required string CreatorUserId { get; set; }

        [Required(ErrorMessage = "El oponente es requerido.")]
        public required string OpponentUserId { get; set; }

        [Required(ErrorMessage = "El estado es requerido.")]
        public BattleshipGameStatus Status { get; set; } = BattleshipGameStatus.Setup;

        [Required(ErrorMessage = "El estado de configuracion del creador es requerido.")]
        public PlayerSetupStatus CreatorSetupStatus { get; set; } = PlayerSetupStatus.NotStarted;

        [Required(ErrorMessage = "El estado de configuracion del oponente es requerido.")]
        public PlayerSetupStatus OpponentSetupStatus { get; set; } = PlayerSetupStatus.NotStarted;

        public string? CurrentTurnUserId { get; set; }

        // ganador final (null hasta que termine)
        public string? WinnerId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // cuando ambos terminan setup
        public DateTime? StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        // para detectar abandono 48h
        public DateTime? LastActivityAt { get; set; }

        public ICollection<Ship> Ships { get; set; } = new List<Ship>();
        public ICollection<Attack> Attacks { get; set; } = new List<Attack>();

    }
}
