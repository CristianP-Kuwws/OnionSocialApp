using LinkUpApp.Core.Domain.Common.Enum.Battleship;
using System.ComponentModel.DataAnnotations;

namespace LinkUpApp.Core.Application.ViewModels.Battleship.Ship
{
    public class SelectShipViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un barco.")]
        public ShipType? SelectedShipType { get; set; }
    }

}
