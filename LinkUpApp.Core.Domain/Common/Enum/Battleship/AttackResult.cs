namespace LinkUpApp.Core.Domain.Common.Enum.Battleship
{
    public enum AttackResult
    {
        Miss, // no habia barco
        Hit,  // habia barco pero no se hundio
        Sunk  // el ataque hundio el barco completo
    }
}
