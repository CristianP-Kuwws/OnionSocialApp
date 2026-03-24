using LinkUpApp.Core.Domain.Common.Enum.Battleship;

namespace LinkUpApp.Core.Application.Dtos.Battleship
{
    public class BattleshipGameDto
    {
        public int Id { get; set; }
        public required string CreatorUserId { get; set; }
        public required string OpponentUserId { get; set; }
        public required BattleshipGameStatus Status { get; set; }
        public required PlayerSetupStatus CreatorSetupStatus { get; set; }
        public required PlayerSetupStatus OpponentSetupStatus { get; set; }
        public string? CurrentTurnUserId { get; set; } 
        public string? WinnerId { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; } 
        public DateTime? FinishedAt { get; set; } 
        public DateTime? LastActivityAt { get; set; } 
    }
}
