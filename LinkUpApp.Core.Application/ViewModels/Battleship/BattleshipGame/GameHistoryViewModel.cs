namespace LinkUpApp.Core.Application.ViewModels.Battleship.BattleshipGame
{
    public class GameHistoryViewModel
    {
        public int GameId { get; set; }
        public required string OpponentUserName { get; set; }
        public required string OpponentFullName { get; set; }
        public string? OpponentProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public int DurationInHours { get; set; }
        public bool DidIWin { get; set; }
        public required string WinnerDisplay { get; set; }
    }
}
