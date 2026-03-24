using LinkUpApp.Core.Domain.Common.Enum.Battleship;
using System.ComponentModel.DataAnnotations;

namespace LinkUpApp.Core.Application.ViewModels.Battleship.Ship
{
    public class PositionShipViewModel
    {
        [Required]
        public int GameId { get; set; }

        [Required]
        public ShipType ShipType { get; set; }

        [Required]
        public int StartRow { get; set; }

        [Required]
        public int StartColumn { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una direccion.")]
        public ShipOrientation? Orientation { get; set; }
    }
}
