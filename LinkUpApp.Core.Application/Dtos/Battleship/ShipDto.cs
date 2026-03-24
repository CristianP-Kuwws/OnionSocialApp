using LinkUpApp.Core.Domain.Common.Enum.Battleship;

namespace LinkUpApp.Core.Application.Dtos.Battleship
{
    public class ShipDto
    {
        public int Id { get; set; }
        public required int GameId { get; set; }
        public required string UserId { get; set; }
        public required ShipType Type { get; set; }
        public required int StartRow { get; set; }
        public required int StartColumn { get; set; }
        public required ShipOrientation Orientation { get; set; }
        public bool IsSunk { get; set; }
        public DateTime PositionedAt { get; set; }
    }
}
