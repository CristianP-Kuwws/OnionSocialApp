using AutoMapper;
using LinkUpApp.Core.Application.Dtos.Battleship;
using LinkUpApp.Core.Application.Interfaces.Battleship;
using LinkUpApp.Core.Application.Interfaces.Social;
using LinkUpApp.Core.Domain.Common.Enum.Battleship;
using LinkUpApp.Core.Domain.Entities.Battleship;
using LinkUpApp.Core.Domain.Interfaces.Battleship;
using Microsoft.EntityFrameworkCore;

namespace LinkUpApp.Core.Application.Services.Battleship
{
    public class BattleshipService : IBattleshipService
    {
        private readonly IBattleshipGameRepository _gameRepository;
        private readonly IShipRepository _shipRepository;
        private readonly IAttackRepository _attackRepository;
        private readonly IFriendshipService _friendshipService;
        private readonly IMapper _mapper;

        public BattleshipService(
            IBattleshipGameRepository gameRepository,
            IShipRepository shipRepository,
            IAttackRepository attackRepository,
            IFriendshipService friendshipService,
            IMapper mapper)
        {
            _gameRepository = gameRepository;
            _shipRepository = shipRepository;
            _attackRepository = attackRepository;
            _friendshipService = friendshipService;
            _mapper = mapper;
        }

        // ============================================
        // Gestion de partidas
        // ============================================

        public async Task<BattleshipGameDto?> CreateGameAsync(string creatorId, string opponentId)
        {
            try
            {
                // validar que sean amigos
                if (!await _friendshipService.AreFriendsAsync(creatorId, opponentId))
                {
                    Console.WriteLine("Error: Los usuarios no son amigos");
                    throw new InvalidOperationException("Solo puedes jugar con tus amigos");
                }

                // validar que no exista partida activa entre ellos
                var query = _gameRepository.GetAllQuery();
                var activeGame = await query.FirstOrDefaultAsync(g =>
                    ((g.CreatorUserId == creatorId && g.OpponentUserId == opponentId) ||
                     (g.CreatorUserId == opponentId && g.OpponentUserId == creatorId)) &&
                    (g.Status == BattleshipGameStatus.Setup || g.Status == BattleshipGameStatus.InProgress));

                if (activeGame != null)
                {
                    Console.WriteLine("Error: Ya existe una partida activa entre estos usuarios");
                    throw new InvalidOperationException("Ya existe una partida activa con este usuario");
                }

                // crear partida
                var game = new BattleshipGame
                {
                    CreatorUserId = creatorId,
                    OpponentUserId = opponentId,
                    Status = BattleshipGameStatus.Setup,
                    CreatorSetupStatus = PlayerSetupStatus.NotStarted,
                    OpponentSetupStatus = PlayerSetupStatus.NotStarted,
                    CreatedAt = DateTime.UtcNow,
                    LastActivityAt = DateTime.UtcNow
                };

                var createdGame = await _gameRepository.AddAsync(game);
                return _mapper.Map<BattleshipGameDto>(createdGame);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateGameAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<BattleshipGameDto>> GetActiveGamesAsync(string userId)
        {
            try
            {
                var query = _gameRepository.GetAllQuery();
                var games = await query
                    .Where(g => (g.CreatorUserId == userId || g.OpponentUserId == userId) &&
                               (g.Status == BattleshipGameStatus.Setup || g.Status == BattleshipGameStatus.InProgress))
                    .OrderByDescending(g => g.CreatedAt)
                    .ToListAsync();

                return _mapper.Map<List<BattleshipGameDto>>(games);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetActiveGamesAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<BattleshipGameDto>> GetFinishedGamesAsync(string userId)
        {
            try
            {
                var query = _gameRepository.GetAllQuery();
                var games = await query
                    .Where(g => (g.CreatorUserId == userId || g.OpponentUserId == userId) &&
                               g.Status == BattleshipGameStatus.Finished)
                    .OrderByDescending(g => g.FinishedAt)
                    .ToListAsync();

                return _mapper.Map<List<BattleshipGameDto>>(games);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetFinishedGamesAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<BattleshipGameDto?> GetGameByIdAsync(int gameId)
        {
            try
            {
                var game = await _gameRepository.GetByIdAsync(gameId);
                return game != null ? _mapper.Map<BattleshipGameDto>(game) : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetGameByIdAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> SurrenderAsync(int gameId, string userId)
        {
            try
            {
                var game = await _gameRepository.GetByIdAsync(gameId);

                if (game == null)
                {
                    Console.WriteLine("Error: Partida no encontrada");
                    throw new InvalidOperationException("La partida no existe");
                }

                if (game.Status == BattleshipGameStatus.Finished)
                {
                    Console.WriteLine("Error: La partida ya termino");
                    throw new InvalidOperationException("La partida ya ha terminado");
                }

                if (game.CreatorUserId != userId && game.OpponentUserId != userId)
                {
                    Console.WriteLine("Error: Usuario no es parte de esta partida");
                    throw new InvalidOperationException("No eres parte de esta partida");
                }

                // determinar ganador (el otro jugador)
                game.WinnerId = game.CreatorUserId == userId ? game.OpponentUserId : game.CreatorUserId;
                game.Status = BattleshipGameStatus.Finished;
                game.FinishedAt = DateTime.UtcNow;

                await _gameRepository.UpdateAsync(gameId, game);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en SurrenderAsync: {ex.Message}");
                throw;
            }
        }

        // ============================================
        // Configuracion de barcos
        // ============================================

        public async Task<List<ShipType>> GetUnpositionedShipsAsync(int gameId, string userId)
        {
            try
            {
                var query = _shipRepository.GetAllQuery();
                var positionedShips = await query
                    .Where(s => s.GameId == gameId && s.UserId == userId)
                    .Select(s => s.Type)
                    .ToListAsync();

                var requiredShips = new List<ShipType>
                {
                    ShipType.Destroyer,  // 2 posiciones
                    ShipType.Submarine,  // 3 posiciones
                    ShipType.Submarine,  // 3 posiciones (segundo)
                    ShipType.Battleship, // 4 posiciones
                    ShipType.Carrier     // 5 posiciones
                };

                // remover los ya posicionados
                foreach (var positioned in positionedShips)
                {
                    requiredShips.Remove(positioned);
                }

                return requiredShips;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetUnpositionedShipsAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CanPositionShipAsync(int gameId, string userId, ShipType type, int startRow, int startCol, ShipOrientation orientation)
        {
            try
            {
                // validar limites del tablero
                if (!IsWithinBounds(type, startRow, startCol, orientation))
                    return false;

                // obtener barcos ya posicionados del usuario
                var query = _shipRepository.GetAllQuery();
                var existingShips = await query
                    .Where(s => s.GameId == gameId && s.UserId == userId)
                    .ToListAsync();

                // validar que no se superpongan con otros barcos
                var newShipCells = GetShipCells(type, startRow, startCol, orientation);
                foreach (var ship in existingShips)
                {
                    var existingCells = GetShipCells(ship.Type, ship.StartRow, ship.StartColumn, ship.Orientation);
                    if (newShipCells.Intersect(existingCells).Any())
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CanPositionShipAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> PositionShipAsync(int gameId, string userId, ShipType type, int startRow, int startCol, ShipOrientation orientation)
        {
            try
            {
                var game = await _gameRepository.GetByIdAsync(gameId);

                if (game == null)
                {
                    Console.WriteLine("Error: Partida no encontrada");
                    throw new InvalidOperationException("La partida no existe");
                }

                if (game.Status != BattleshipGameStatus.Setup)
                {
                    Console.WriteLine("Error: La fase de configuracion ya termino");
                    throw new InvalidOperationException("La fase de configuracion ya ha terminado");
                }

                // validar que no haya 5 barcos ya
                var unpositioned = await GetUnpositionedShipsAsync(gameId, userId);
                if (unpositioned.Count == 0)
                {
                    Console.WriteLine("Error: Ya se han posicionado los 5 barcos");
                    throw new InvalidOperationException("Ya has posicionado todos tus barcos");
                }

                // validar que el tipo de barco este disponible
                if (!unpositioned.Contains(type))
                {
                    Console.WriteLine($"Error: El barco de tipo {type} ya fue posicionado");
                    throw new InvalidOperationException("Este tipo de barco ya fue posicionado");
                }

                // validar posicion
                if (!await CanPositionShipAsync(gameId, userId, type, startRow, startCol, orientation))
                {
                    Console.WriteLine("Error: Posicion invalida");
                    throw new InvalidOperationException("La posicion seleccionada no es valida");
                }

                // crear barco
                var ship = new Ship
                {
                    GameId = gameId,
                    UserId = userId,
                    Type = type,
                    StartRow = startRow,
                    StartColumn = startCol,
                    Orientation = orientation,
                    IsSunk = false,
                    PositionedAt = DateTime.UtcNow
                };

                await _shipRepository.AddAsync(ship);

                // verificar si completo el setup
                var stillUnpositioned = await GetUnpositionedShipsAsync(gameId, userId);
                if (stillUnpositioned.Count == 0)
                {
                    // actualizar estado del jugador
                    if (game.CreatorUserId == userId)
                        game.CreatorSetupStatus = PlayerSetupStatus.Completed;
                    else
                        game.OpponentSetupStatus = PlayerSetupStatus.Completed;

                    // si ambos completaron, iniciar partida
                    if (game.CreatorSetupStatus == PlayerSetupStatus.Completed &&
                        game.OpponentSetupStatus == PlayerSetupStatus.Completed)
                    {
                        game.Status = BattleshipGameStatus.InProgress;
                        game.CurrentTurnUserId = game.CreatorUserId; // el creador ataca primero
                        game.StartedAt = DateTime.UtcNow;
                        game.LastActivityAt = DateTime.UtcNow;
                    }

                    await _gameRepository.UpdateAsync(gameId, game);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en PositionShipAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> IsSetupCompleteAsync(int gameId, string userId)
        {
            try
            {
                var unpositioned = await GetUnpositionedShipsAsync(gameId, userId);
                return unpositioned.Count == 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en IsSetupCompleteAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<ShipDto>> GetUserShipsAsync(int gameId, string userId)
        {
            try
            {
                var query = _shipRepository.GetAllQuery();
                var ships = await query
                    .Where(s => s.GameId == gameId && s.UserId == userId)
                    .ToListAsync();

                return _mapper.Map<List<ShipDto>>(ships);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetUserShipsAsync: {ex.Message}");
                throw;
            }
        }

        // ============================================
        // Ataques
        // ============================================

        public async Task<AttackDto?> AttackAsync(int gameId, string userId, int targetRow, int targetCol)
        {
            try
            {
                var game = await _gameRepository.GetByIdAsync(gameId);

                if (game == null)
                {
                    Console.WriteLine("Error: Partida no encontrada");
                    throw new InvalidOperationException("La partida no existe");
                }

                if (game.Status != BattleshipGameStatus.InProgress)
                {
                    Console.WriteLine("Error: La partida no esta en progreso");
                    throw new InvalidOperationException("La partida no esta en progreso");
                }

                // validar que sea el turno del usuario
                if (!await IsUserTurnAsync(gameId, userId))
                {
                    Console.WriteLine("Error: No es tu turno");
                    throw new InvalidOperationException("No es tu turno");
                }

                // validar que la celda no haya sido atacada
                var attackQuery = _attackRepository.GetAllQuery();
                var alreadyAttacked = await attackQuery
                    .AnyAsync(a => a.GameId == gameId &&
                                  a.AttackingUserId == userId &&
                                  a.TargetRow == targetRow &&
                                  a.TargetColumn == targetCol);

                if (alreadyAttacked)
                {
                    Console.WriteLine("Error: Esta celda ya fue atacada");
                    throw new InvalidOperationException("Esta celda ya fue atacada");
                }

                // determinar oponente
                var opponentId = game.CreatorUserId == userId ? game.OpponentUserId : game.CreatorUserId;

                // verificar si hay un barco del oponente en esa posicion
                var shipQuery = _shipRepository.GetAllQuery();
                var opponentShips = await shipQuery
                    .Where(s => s.GameId == gameId && s.UserId == opponentId)
                    .ToListAsync();

                bool hit = false;
                Ship? hitShip = null;

                foreach (var ship in opponentShips)
                {
                    var shipCells = GetShipCells(ship.Type, ship.StartRow, ship.StartColumn, ship.Orientation);
                    if (shipCells.Any(c => c.row == targetRow && c.col == targetCol))
                    {
                        hit = true;
                        hitShip = ship;
                        break;
                    }
                }

                // determinar numero de turno
                var turnNumber = await attackQuery.CountAsync(a => a.GameId == gameId) + 1;

                // crear ataque
                var attack = new Attack
                {
                    GameId = gameId,
                    AttackingUserId = userId,
                    TargetRow = targetRow,
                    TargetColumn = targetCol,
                    Result = hit ? AttackResult.Hit : AttackResult.Miss,
                    TurnNumber = turnNumber,
                    CreatedAt = DateTime.UtcNow
                };

                var createdAttack = await _attackRepository.AddAsync(attack);

                // si hubo un hit, verificar si el barco se hundio
                if (hit && hitShip != null)
                {
                    var shipCells = GetShipCells(hitShip.Type, hitShip.StartRow, hitShip.StartColumn, hitShip.Orientation);
                    var hitsOnShip = await attackQuery
                        .Where(a => a.GameId == gameId && a.Result == AttackResult.Hit)
                        .ToListAsync();

                    var allCellsHit = shipCells.All(cell =>
                        hitsOnShip.Any(a => a.TargetRow == cell.row && a.TargetColumn == cell.col));

                    if (allCellsHit)
                    {
                        hitShip.IsSunk = true;
                        await _shipRepository.UpdateAsync(hitShip.Id, hitShip);
                        attack.Result = AttackResult.Sunk;
                        await _attackRepository.UpdateAsync(createdAttack!.Id, attack);
                    }
                }

                // cambiar turno
                game.CurrentTurnUserId = opponentId;
                game.LastActivityAt = DateTime.UtcNow;
                await _gameRepository.UpdateAsync(gameId, game);

                // verificar victoria
                await CheckVictoryAsync(gameId);

                return _mapper.Map<AttackDto>(createdAttack);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AttackAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> IsUserTurnAsync(int gameId, string userId)
        {
            try
            {
                var game = await _gameRepository.GetByIdAsync(gameId);
                return game?.CurrentTurnUserId == userId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en IsUserTurnAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<AttackDto>> GetUserAttacksAsync(int gameId, string userId)
        {
            try
            {
                var query = _attackRepository.GetAllQuery();
                var attacks = await query
                    .Where(a => a.GameId == gameId && a.AttackingUserId == userId)
                    .OrderBy(a => a.TurnNumber)
                    .ToListAsync();

                return _mapper.Map<List<AttackDto>>(attacks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetUserAttacksAsync: {ex.Message}");
                throw;
            }
        }

        // ============================================
        // ESTADO DEL JUEGO
        // ============================================

        public async Task<bool> CheckVictoryAsync(int gameId)
        {
            try
            {
                var game = await _gameRepository.GetByIdAsync(gameId);

                if (game == null || game.Status != BattleshipGameStatus.InProgress)
                    return false;

                // verificar si todos los barcos de algun jugador estan hundidos
                var shipQuery = _shipRepository.GetAllQuery();

                var creatorShips = await shipQuery
                    .Where(s => s.GameId == gameId && s.UserId == game.CreatorUserId)
                    .ToListAsync();

                var opponentShips = await shipQuery
                    .Where(s => s.GameId == gameId && s.UserId == game.OpponentUserId)
                    .ToListAsync();

                bool creatorDefeated = creatorShips.All(s => s.IsSunk);
                bool opponentDefeated = opponentShips.All(s => s.IsSunk);

                if (creatorDefeated || opponentDefeated)
                {
                    game.Status = BattleshipGameStatus.Finished;
                    game.WinnerId = creatorDefeated ? game.OpponentUserId : game.CreatorUserId;
                    game.FinishedAt = DateTime.UtcNow;
                    await _gameRepository.UpdateAsync(gameId, game);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CheckVictoryAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<(int won, int lost, int total)> GetUserStatsAsync(string userId)
        {
            try
            {
                var query = _gameRepository.GetAllQuery();
                var finishedGames = await query
                    .Where(g => (g.CreatorUserId == userId || g.OpponentUserId == userId) &&
                               g.Status == BattleshipGameStatus.Finished)
                    .ToListAsync();

                var total = finishedGames.Count;
                var won = finishedGames.Count(g => g.WinnerId == userId);
                var lost = total - won;

                return (won, lost, total);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetUserStatsAsync: {ex.Message}");
                throw;
            }
        }

        // ============================================
        // Abandono automatico
        // ============================================

        public async Task CheckAndHandleAbandonedGamesAsync()
        {
            try
            {
                var cutoffTime = DateTime.UtcNow.AddHours(-48);

                var query = _gameRepository.GetAllQuery();
                var abandonedGames = await query
                    .Where(g => g.Status == BattleshipGameStatus.InProgress &&
                               g.LastActivityAt < cutoffTime)
                    .ToListAsync();

                foreach (var game in abandonedGames)
                {
                    // el ganador es el jugador que no tiene el turno actual
                    game.WinnerId = game.CurrentTurnUserId == game.CreatorUserId
                        ? game.OpponentUserId
                        : game.CreatorUserId;
                    game.Status = BattleshipGameStatus.Finished;
                    game.FinishedAt = DateTime.UtcNow;

                    await _gameRepository.UpdateAsync(game.Id, game);
                }

                Console.WriteLine($"Se finalizaron {abandonedGames.Count} partidas abandonadas");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CheckAndHandleAbandonedGamesAsync: {ex.Message}");
                throw;
            }
        }

        // ============================================
        // Metodos Privados
        // ============================================

        private bool IsWithinBounds(ShipType type, int startRow, int startCol, ShipOrientation orientation) // . . .
        {
            var size = (int)type;
            var (endRow, endCol) = orientation switch
            {
                ShipOrientation.Up => (startRow - size + 1, startCol),
                ShipOrientation.Down => (startRow + size - 1, startCol),
                ShipOrientation.Left => (startRow, startCol - size + 1),
                ShipOrientation.Right => (startRow, startCol + size - 1),
                _ => (startRow, startCol)
            };

            return endRow >= 0 && endRow < 12 && endCol >= 0 && endCol < 12 &&
                   startRow >= 0 && startRow < 12 && startCol >= 0 && startCol < 12;
        }

        private List<(int row, int col)> GetShipCells(ShipType type, int startRow, int startCol, ShipOrientation orientation)
        {
            var cells = new List<(int, int)>();
            var size = (int)type;

            for (int i = 0; i < size; i++)
            {
                var (row, col) = orientation switch
                {
                    ShipOrientation.Up => (startRow - i, startCol),
                    ShipOrientation.Down => (startRow + i, startCol),
                    ShipOrientation.Left => (startRow, startCol - i),
                    ShipOrientation.Right => (startRow, startCol + i),
                    _ => (startRow, startCol)
                };
                cells.Add((row, col));
            }

            return cells;
        }
    }
}
