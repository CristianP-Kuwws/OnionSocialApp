using LinkUpApp.Core.Domain.Common.Enum.Battleship;
using System.ComponentModel.DataAnnotations;

namespace LinkUpApp.Core.Domain.Entities.Battleship
{
    public class Ship
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La partida es requerida.")]
        public required int GameId { get; set; }

        [Required(ErrorMessage = "El usuario es requerido.")]
        public required string UserId { get; set; }

        [Required(ErrorMessage = "El tipo de barco es requerido.")]
        public ShipType Type { get; set; }

        [Required(ErrorMessage = "La fila inicial es requerida.")]
        [Range(0, 11, ErrorMessage = "La fila debe estar entre 0 y 11.")]
        public required int StartRow { get; set; }

        [Required(ErrorMessage = "La columna inicial es requerida.")]
        [Range(0, 11, ErrorMessage = "La columna debe estar entre 0 y 11.")]
        public required int StartColumn { get; set; }

        [Required(ErrorMessage = "La orientacion es requerida.")]
        public ShipOrientation Orientation { get; set; }

        public bool IsSunk { get; set; } = false; // se actualiza cuando todas sus posiciones son atacadas

        public DateTime PositionedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public BattleshipGame Game { get; set; } = null!;

    }
}
