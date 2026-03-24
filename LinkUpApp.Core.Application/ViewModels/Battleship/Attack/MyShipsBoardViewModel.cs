using LinkUpApp.Core.Domain.Common.Enum.Battleship.CellStates;

namespace LinkUpApp.Core.Application.ViewModels.Battleship.Attack
{
    public class MyShipsBoardViewModel
    {
        public int GameId { get; set; }
        public ShipPositionCellState[,] Board { get; set; } = new ShipPositionCellState[12, 12];
    }
}
