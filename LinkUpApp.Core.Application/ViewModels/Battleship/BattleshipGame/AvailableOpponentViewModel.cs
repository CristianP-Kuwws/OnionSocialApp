namespace LinkUpApp.Core.Application.ViewModels.Battleship.BattleshipGame
{
    public class AvailableOpponentViewModel
    {
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public required string FullName { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
