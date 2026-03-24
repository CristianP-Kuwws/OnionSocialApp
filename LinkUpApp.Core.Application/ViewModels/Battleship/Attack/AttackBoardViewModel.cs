using LinkUpApp.Core.Domain.Common.Enum.Battleship.CellStates;

namespace LinkUpApp.Core.Application.ViewModels.Battleship.Attack
{
    public class AttackBoardViewModel
    {
        public int GameId { get; set; }
        public AttackCellState[,] Board { get; set; } = new AttackCellState[12, 12];
        public bool IsMyTurn { get; set; }
        public required string CurrentTurnPlayerName { get; set; }
        public int TurnNumber { get; set; }
    }
}
