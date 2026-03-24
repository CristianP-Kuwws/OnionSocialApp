using LinkUpApp.Core.Domain.Common.Enum.Battleship;

namespace LinkUpApp.Core.Application.ViewModels.Battleship.Ship
{
    public class AvailableShipViewModel
    {
        public required ShipType Type { get; set; }
        public required int Size { get; set; }
        public int Quantity { get; set; }
    }
}
