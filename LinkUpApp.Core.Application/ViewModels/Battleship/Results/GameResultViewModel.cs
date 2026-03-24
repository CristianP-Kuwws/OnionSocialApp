using LinkUpApp.Core.Domain.Common.Enum.Battleship.CellStates;

namespace LinkUpApp.Core.Application.ViewModels.Battleship.Results
{
    public class GameResultViewModel
    {
        public int GameId { get; set; }
        public required string OpponentUserName { get; set; }
        public bool DidIWin { get; set; }
        public AttackCellState[,] MyAttackBoard { get; set; } = new AttackCellState[12, 12];
        public AttackCellState[,] OpponentAttackBoard { get; set; } = new AttackCellState[12, 12];
        public ShipPositionCellState[,] MyShipBoard { get; set; } = new ShipPositionCellState[12, 12];
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public int DurationInHours { get; set; }
    }
}
