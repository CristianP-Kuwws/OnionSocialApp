using LinkUpApp.Core.Domain.Common.Enum.Battleship;

namespace LinkUpApp.Core.Application.ViewModels.Battleship.BattleshipGame
{
    public class ActiveGameViewModel
    {
        public int GameId { get; set; }
        public required string OpponentUserName { get; set; }
        public required string OpponentFullName { get; set; }
        public string? OpponentProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; }
        public int HoursElapsed { get; set; }
        public required PlayerSetupStatus MySetupStatus { get; set; }
        public required PlayerSetupStatus OpponentSetupStatus { get; set; }
    }

}
