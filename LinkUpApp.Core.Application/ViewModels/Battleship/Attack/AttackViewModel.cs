namespace LinkUpApp.Core.Application.ViewModels.Battleship.Attack
{
    public class AttackViewModel
    {
        public required int GameId { get; set; }
        public required int TargetRow { get; set; }
        public required int TargetColumn { get; set; }
    }
}
