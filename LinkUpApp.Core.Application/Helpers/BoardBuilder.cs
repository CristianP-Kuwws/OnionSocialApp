using LinkUpApp.Core.Application.Dtos.Battleship;
using LinkUpApp.Core.Application.Interfaces.Helpers;
using LinkUpApp.Core.Domain.Common.Enum.Battleship;
using LinkUpApp.Core.Domain.Common.Enum.Battleship.CellStates;

namespace LinkUpApp.Core.Application.Helpers
{
    public class BoardBuilder : IBoardBuilder
    {
        private const int BoardSize = 12;

        public CellState[,] CreateEmptyBoard()
        {
            var board = new CellState[BoardSize, BoardSize];
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    board[i, j] = CellState.Empty;
                }
            }
            return board;
        }

        public CellState[,] BuildBoardWithShips(List<ShipDto> ships)
        {
            var board = CreateEmptyBoard();

            foreach (var ship in ships)
            {
                var cells = GetShipCells(ship.Type, ship.StartRow, ship.StartColumn, ship.Orientation);
                foreach (var (row, col) in cells)
                {
                    if (IsValidPosition(row, col))
                    {
                        board[row, col] = CellState.Occupied;
                    }
                }
            }

            return board;
        }

        public AttackCellState[,] BuildAttackBoard(List<AttackDto> attacks)
        {
            var board = new AttackCellState[BoardSize, BoardSize];

            // Inicializar tablero
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    board[i, j] = AttackCellState.Unknown;
                }
            }

            // Marcar ataques
            foreach (var attack in attacks)
            {
                if (IsValidPosition(attack.TargetRow, attack.TargetColumn))
                {
                    board[attack.TargetRow, attack.TargetColumn] =
                        attack.Result == AttackResult.Hit || attack.Result == AttackResult.Sunk
                            ? AttackCellState.Hit
                            : AttackCellState.Miss;
                }
            }

            return board;
        }

        public ShipPositionCellState[,] BuildShipBoard(List<ShipDto> ships)
        {
            var board = new ShipPositionCellState[BoardSize, BoardSize];

            // Inicializar tablero
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    board[i, j] = ShipPositionCellState.Empty;
                }
            }

            // Marcar barcos
            foreach (var ship in ships)
            {
                var cells = GetShipCells(ship.Type, ship.StartRow, ship.StartColumn, ship.Orientation);
                foreach (var (row, col) in cells)
                {
                    if (IsValidPosition(row, col))
                    {
                        board[row, col] = ShipPositionCellState.Ship;
                    }
                }
            }

            return board;
        }

        public ShipPositionCellState[,] BuildShipBoardWithHits(
            List<ShipDto> ships,
            List<AttackDto> opponentAttacks)
        {
            var board = new ShipPositionCellState[BoardSize, BoardSize];

            // Inicializar tablero
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    board[i, j] = ShipPositionCellState.Empty;
                }
            }

            // Marcar y verificar hits
            foreach (var ship in ships)
            {
                var cells = GetShipCells(ship.Type, ship.StartRow, ship.StartColumn, ship.Orientation);
                foreach (var (row, col) in cells)
                {
                    if (IsValidPosition(row, col))
                    {
                        bool wasHit = opponentAttacks.Any(a =>
                            a.TargetRow == row &&
                            a.TargetColumn == col &&
                            (a.Result == AttackResult.Hit || a.Result == AttackResult.Sunk));

                        board[row, col] = wasHit
                            ? ShipPositionCellState.ShipHit
                            : ShipPositionCellState.Ship;
                    }
                }
            }

            return board;
        }

        private bool IsValidPosition(int row, int col)
        {
            return row >= 0 && row < BoardSize && col >= 0 && col < BoardSize;
        }

        public List<(int row, int col)> GetShipCells(
            ShipType type,
            int startRow,
            int startCol,
            ShipOrientation orientation)
        {
            var cells = new List<(int, int)>();
            var size = (int)type;

            for (int i = 0; i < size; i++)
            {
                var (row, col) = orientation switch
                {
                    ShipOrientation.Up => (startRow - i, startCol),
                    ShipOrientation.Down => (startRow + i, startCol),
                    ShipOrientation.Left => (startRow, startCol - i),
                    ShipOrientation.Right => (startRow, startCol + i),
                    _ => (startRow, startCol)
                };
                cells.Add((row, col));
            }

            return cells;
        }
    }
}
