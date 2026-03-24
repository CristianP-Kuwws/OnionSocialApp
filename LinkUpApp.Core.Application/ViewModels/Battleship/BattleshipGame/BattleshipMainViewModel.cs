namespace LinkUpApp.Core.Application.ViewModels.Battleship.BattleshipGame
{
    public class BattleshipMainViewModel
    {
        public List<ActiveGameViewModel> ActiveGames { get; set; } = new();
        public List<GameHistoryViewModel> GameHistory { get; set; } = new();
        public GameStatisticsViewModel Statistics { get; set; } = new();
    }
}
