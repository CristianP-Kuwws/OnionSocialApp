using LinkUpApp.Core.Domain.Common.Enum.Battleship;

namespace LinkUpApp.Core.Application.ViewModels.Battleship.Ship
{
    public class SelectCellViewModel
    {
        public int GameId { get; set; }
        public required ShipType ShipType { get; set; }
        public required int SelectedRow { get; set; }
        public required int SelectedColumn { get; set; }
    }
}
