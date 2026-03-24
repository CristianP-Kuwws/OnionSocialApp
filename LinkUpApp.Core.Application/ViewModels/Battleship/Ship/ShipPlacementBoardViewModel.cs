using LinkUpApp.Core.Domain.Common.Enum.Battleship.CellStates;

namespace LinkUpApp.Core.Application.ViewModels.Battleship.Ship
{
    public class ShipPlacementBoardViewModel
    {
        public int GameId { get; set; }
        public CellState[,] Board { get; set; } = new CellState[12, 12];
        public List<AvailableShipViewModel> RemainingShips { get; set; } = new();
        public bool AllShipsPlaced { get; set; }
        public bool OpponentReady { get; set; }
    }
}
