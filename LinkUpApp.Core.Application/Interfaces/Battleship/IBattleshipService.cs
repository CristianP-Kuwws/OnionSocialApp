using LinkUpApp.Core.Application.Dtos.Battleship;
using LinkUpApp.Core.Domain.Common.Enum.Battleship;

namespace LinkUpApp.Core.Application.Interfaces.Battleship
{
    public interface IBattleshipService
    {
        // Gestionar partidas
        Task<BattleshipGameDto?> CreateGameAsync(string creatorId, string opponentId);
        Task<List<BattleshipGameDto>> GetActiveGamesAsync(string userId);
        Task<List<BattleshipGameDto>> GetFinishedGamesAsync(string userId);
        Task<BattleshipGameDto?> GetGameByIdAsync(int gameId);
        Task<bool> SurrenderAsync(int gameId, string userId);

        // Configurar barcos
        Task<List<ShipType>> GetUnpositionedShipsAsync(int gameId, string userId);
        Task<bool> PositionShipAsync(int gameId, string userId, ShipType type, int startRow, int startCol, ShipOrientation orientation);
        Task<bool> IsSetupCompleteAsync(int gameId, string userId);
        Task<List<ShipDto>> GetUserShipsAsync(int gameId, string userId);

        // Validar posiciones
        Task<bool> CanPositionShipAsync(int gameId, string userId, ShipType type, int startRow, int startCol, ShipOrientation orientation);

        // Ataques
        Task<AttackDto?> AttackAsync(int gameId, string userId, int targetRow, int targetCol);
        Task<bool> IsUserTurnAsync(int gameId, string userId);
        Task<List<AttackDto>> GetUserAttacksAsync(int gameId, string userId);

        // Estado del juego
        Task<bool> CheckVictoryAsync(int gameId);
        Task<(int won, int lost, int total)> GetUserStatsAsync(string userId);

        // Abandono automatico
        Task CheckAndHandleAbandonedGamesAsync();
    }
}
