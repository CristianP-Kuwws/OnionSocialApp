namespace LinkUpApp.Core.Domain.Common.Enum.Battleship.CellStates
{
    public enum AttackCellState
    {
        Unknown,      // No atacada
        Hit,          // Atacada - habia barco (rojo)
        Miss,         // Atacada - no habia barco (verde)
        AlreadyHit    // Ya fue atacada antes
    }
}
