using LinkUpApp.Core.Domain.Common.Enum.Battleship;

namespace LinkUpApp.Core.Application.Dtos.Battleship
{
    public class AttackDto
    {
        public int Id { get; set; }
        public required int GameId { get; set; }
        public required string AttackingUserId { get; set; }
        public required int TargetRow { get; set; }
        public required int TargetColumn { get; set; }
        public required AttackResult Result { get; set; }
        public required int TurnNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
