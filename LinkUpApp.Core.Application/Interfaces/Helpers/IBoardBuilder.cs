using LinkUpApp.Core.Application.Dtos.Battleship;
using LinkUpApp.Core.Domain.Common.Enum.Battleship;
using LinkUpApp.Core.Domain.Common.Enum.Battleship.CellStates;

namespace LinkUpApp.Core.Application.Interfaces.Helpers
{
    public interface IBoardBuilder
    {
        CellState[,] CreateEmptyBoard();
        CellState[,] BuildBoardWithShips(List<ShipDto> ships);
        AttackCellState[,] BuildAttackBoard(List<AttackDto> attacks);
        ShipPositionCellState[,] BuildShipBoard(List<ShipDto> ships);
        ShipPositionCellState[,] BuildShipBoardWithHits(
            List<ShipDto> ships,
            List<AttackDto> opponentAttacks);
        List<(int row, int col)> GetShipCells(
            ShipType type,
            int startRow,
            int startCol,
            ShipOrientation orientation);
    }
}
