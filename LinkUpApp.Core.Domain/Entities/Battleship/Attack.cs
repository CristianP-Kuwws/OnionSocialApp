using LinkUpApp.Core.Domain.Common.Enum.Battleship;
using System.ComponentModel.DataAnnotations;

namespace LinkUpApp.Core.Domain.Entities.Battleship
{
    public class Attack
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La partida es requerida.")]
        public required int GameId { get; set; }

        [Required(ErrorMessage = "El usuario atacante es requerido.")]
        public required string AttackingUserId { get; set; }

        [Required(ErrorMessage = "La fila objetivo es requerida.")]
        [Range(0, 11, ErrorMessage = "La fila debe estar entre 0 y 11.")]
        public required int TargetRow { get; set; }

        [Required(ErrorMessage = "La columna objetivo es requerida.")]
        [Range(0, 11, ErrorMessage = "La columna debe estar entre 0 y 11.")]
        public required int TargetColumn { get; set; }

        [Required(ErrorMessage = "El resultado es requerido.")]
        public AttackResult Result { get; set; }

        [Required(ErrorMessage = "El numero de turno es requerido.")]
        public required int TurnNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public BattleshipGame Game { get; set; } = null!;
    }
}
