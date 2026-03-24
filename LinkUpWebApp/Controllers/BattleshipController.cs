using LinkUpApp.Core.Application.Interfaces.Battleship;
using LinkUpApp.Core.Application.Interfaces.Helpers;
using LinkUpApp.Core.Application.Interfaces.Social;
using LinkUpApp.Core.Application.Interfaces.User;
using LinkUpApp.Core.Application.ViewModels.Battleship.Attack;
using LinkUpApp.Core.Application.ViewModels.Battleship.BattleshipGame;
using LinkUpApp.Core.Application.ViewModels.Battleship.Ship;
using LinkUpApp.Core.Domain.Common.Enum.Battleship;
using LinkUpApp.Core.Domain.Common.Enum.Battleship.CellStates;
using LinkUpApp.Infrastructure.Identity.Entities;
using LinkUpWebApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpWebApp.Controllers
{
    [Authorize]
    public class BattleshipController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountServicesForWebApp _accountService;
        private readonly IBattleshipService _battleshipService;
        private readonly IFriendshipService _friendshipService;
        private readonly IBoardBuilder _boardBuilder;

        public BattleshipController(
            UserManager<ApplicationUser> userManager,
            IAccountServicesForWebApp accountService,
            IBattleshipService battleshipService,
            IFriendshipService friendshipService,
            IBoardBuilder boardBuilder)
        {
            _userManager = userManager;
            _accountService = accountService;
            _battleshipService = battleshipService;
            _friendshipService = friendshipService;
            _boardBuilder = boardBuilder;
        }

        // ============================================
        // Pantalla principal (Listados)
        // ============================================

        public async Task<IActionResult> Index()
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login");

            var viewModel = new BattleshipMainViewModel();

            // Obtener partidas activas
            var activeGamesDto = await _battleshipService.GetActiveGamesAsync(currentUser.Id);
            viewModel.ActiveGames = await BattleshipMappers.MapActiveGamesAsync(
                activeGamesDto,
                currentUser.Id,
                _accountService);

            // Obtener historial de partidas
            var finishedGamesDto = await _battleshipService.GetFinishedGamesAsync(currentUser.Id);
            viewModel.GameHistory = await BattleshipMappers.MapGameHistoryAsync(
                finishedGamesDto,
                currentUser.Id,
                _accountService);

            // Obtener estadisticas
            var (won, lost, total) = await _battleshipService.GetUserStatsAsync(currentUser.Id);
            viewModel.Statistics = new GameStatisticsViewModel
            {
                TotalGames = total,
                GamesWon = won,
                GamesLost = lost
            };

            return View(viewModel);
        }

        // ============================================
        // Iniciar nueva partida
        // ============================================

        public async Task<IActionResult> SelectOpponent(string searchTerm)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login", new { unauthorized = true });

            // Obtener amistades del usuario logueado
            var friendships = await _friendshipService.GetFriendsAsync(currentUser.Id);

            // Obtener partidas activas
            var activeGames = await _battleshipService.GetActiveGamesAsync(currentUser.Id);

            var availableOpponents = new List<AvailableOpponentViewModel>();

            foreach (var friendship in friendships)
            {
                // Determinar el ID del amigo 
                var friendId = friendship.User1Id == currentUser.Id
                    ? friendship.User2Id
                    : friendship.User1Id;

                // Verificar que no tenga partida activa con este usuario
                bool hasActiveGame = activeGames.Any(g =>
                    g.CreatorUserId == friendId || g.OpponentUserId == friendId);

                if (hasActiveGame)
                    continue;

                // Obtener los datos del amigo
                var friend = await _accountService.GetUserById(friendId);
                if (friend == null)
                    continue;

                availableOpponents.Add(new AvailableOpponentViewModel
                {
                    UserId = friend.Id,
                    UserName = friend.UserName,
                    FullName = $"{friend.FirstName} {friend.LastName}",
                    ProfilePicture = friend.ProfilePicturePath
                });
            }

            // Filtrar 
            if (!string.IsNullOrEmpty(searchTerm))
            {
                availableOpponents = availableOpponents
                    .Where(o => o.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.SearchTerm = searchTerm;

            return View(availableOpponents);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartGame(StartGameViewModel vm)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login");

            if (string.IsNullOrEmpty(vm.SelectedOpponentId))
            {
                TempData["ErrorMessage"] = "Debe seleccionar un oponente.";
                return RedirectToAction("SelectOpponent");
            }

            try
            {
                var gameDto = await _battleshipService.CreateGameAsync(currentUser.Id, vm.SelectedOpponentId);

                if (gameDto != null)
                {
                    // Redirigir a la pantalla de seleccion de barcos
                    return RedirectToAction("SelectShip", new { gameId = gameDto.Id });
                }

                TempData["ErrorMessage"] = "No se pudo crear la partida.";
                return RedirectToAction("SelectOpponent");
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("SelectOpponent");
            }
        }

        // ============================================
        // Rendirse
        // ============================================

        public async Task<IActionResult> ConfirmSurrender(int gameId)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login");

            var gameDto = await _battleshipService.GetGameByIdAsync(gameId);
            if (gameDto == null)
                return RedirectToAction("Index");

            ViewBag.GameId = gameId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Surrender(int gameId)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login");

            try
            {
                await _battleshipService.SurrenderAsync(gameId, currentUser.Id);
                TempData["SuccessMessage"] = "Te has rendido. El oponente gana la partida.";
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // ============================================
        // Entrar a partida activa
        // ============================================

        public async Task<IActionResult> EnterGame(int gameId)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login");

            var gameDto = await _battleshipService.GetGameByIdAsync(gameId);
            if (gameDto == null)
            {
                TempData["ErrorMessage"] = "La partida no existe.";
                return RedirectToAction("Index");
            }

            // Verificar si el usuario ya completo su configuracion 
            bool setupComplete = await _battleshipService.IsSetupCompleteAsync(gameId, currentUser.Id);

            if (!setupComplete)
            {
                // Redirigir a configuracion de barcos
                return RedirectToAction("SelectShip", new { gameId });
            }

            // Verificar si el oponente tambien completo su configuracion
            string opponentId = gameDto.CreatorUserId == currentUser.Id
                ? gameDto.OpponentUserId
                : gameDto.CreatorUserId;

            bool opponentSetupComplete = await _battleshipService.IsSetupCompleteAsync(gameId, opponentId);

            if (!opponentSetupComplete)
            {
                // Mostrar tablero de espera
                return RedirectToAction("WaitingForOpponent", new { gameId });
            }

            return RedirectToAction("AttackBoard", new { gameId });
        }

        // ============================================
        // Configuracion de barcos
        // ============================================

        public async Task<IActionResult> SelectShip(int gameId)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login");

            var gameDto = await _battleshipService.GetGameByIdAsync(gameId);
            if (gameDto == null)
            {
                TempData["ErrorMessage"] = "La partida no existe.";
                return RedirectToAction("Index");
            }

            // Obtener barcos no posicionados
            var unpositionedShips = await _battleshipService.GetUnpositionedShipsAsync(gameId, currentUser.Id);

            if (unpositionedShips.Count == 0)
            {
                // Todos los barcos fueron posicionados
                return RedirectToAction("WaitingForOpponent", new { gameId });
            }

            // Crear lista de barcos disponibles
            var availableShips = unpositionedShips
                .Select(shipType => new AvailableShipViewModel
                {
                    Type = shipType,
                    Size = (int)shipType,
                    Quantity = 1
                })
                .ToList();

            ViewBag.GameId = gameId;
            return View(availableShips);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectShip(int gameId, SelectShipViewModel vm)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login");

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Debe seleccionar un barco.";
                return RedirectToAction("SelectShip", new { gameId });
            }

            // Redirigir a seleccion de celda
            return RedirectToAction("SelectCell", new { gameId, shipType = (int)vm.SelectedShipType });
        }

        public async Task<IActionResult> SelectCell(int gameId, int shipType)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login");

            var gameDto = await _battleshipService.GetGameByIdAsync(gameId);
            if (gameDto == null)
            {
                TempData["ErrorMessage"] = "La partida no existe.";
                return RedirectToAction("Index");
            }

            var validShipSizes = new[] { 2, 3, 4, 5 };
            if (!validShipSizes.Contains(shipType))
            {
                TempData["ErrorMessage"] = "Tipo de barco invalido.";
                return RedirectToAction("SelectShip", new { gameId });
            }
            var shipTypeEnum = (ShipType)shipType;

            // Obtener barcos ya posicionados para mostrar celdas ocupadas
            var userShips = await _battleshipService.GetUserShipsAsync(gameId, currentUser.Id);

            var board = _boardBuilder.BuildBoardWithShips(userShips);

            // Marcar celdas ocupadas por barcos existentes
            foreach (var ship in userShips)
            {
                var cells = _boardBuilder.GetShipCells(ship.Type, ship.StartRow, ship.StartColumn, ship.Orientation);
                foreach (var (row, col) in cells)
                {
                    if (row >= 0 && row < 12 && col >= 0 && col < 12)
                    {
                        board[row, col] = CellState.Occupied;
                    }
                }
            }

            ViewBag.GameId = gameId;
            ViewBag.ShipType = shipTypeEnum;
            ViewBag.ShipSize = shipType;  

            return View(board);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectCell(int gameId, int shipType, int selectedRow, int selectedColumn)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login");

            if (!Enum.IsDefined(typeof(ShipType), shipType))
            {
                TempData["ErrorMessage"] = "Tipo de barco invalido.";
                return RedirectToAction("SelectShip", new { gameId });
            }

            var shipTypeEnum = (ShipType)shipType;

            return RedirectToAction("SelectDirection", new
            {
                gameId,
                shipType = shipType, 
                startRow = selectedRow,
                startCol = selectedColumn
            });
        }

        public async Task<IActionResult> SelectDirection(int gameId, int shipType, int startRow, int startCol)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login");

            // Validar que shipType sea un valor definido (2, 3, 4, o 5)
            if (!Enum.IsDefined(typeof(ShipType), shipType)) 
            {
                TempData["ErrorMessage"] = "Tipo de barco invalido.";
                return RedirectToAction("SelectShip", new { gameId });
            }

            var shipTypeEnum = (ShipType)shipType;

            ViewBag.GameId = gameId;
            ViewBag.ShipType = shipTypeEnum;
            ViewBag.ShipSize = (int)shipTypeEnum;
            ViewBag.StartRow = startRow;
            ViewBag.StartCol = startCol;

            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PositionShip(PositionShipViewModel vm)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login");

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Debe completar todos los campos correctamente.";
                return RedirectToAction("SelectDirection", new
                {
                    gameId = vm.GameId,
                    shipType = (int)vm.ShipType,
                    startRow = vm.StartRow,
                    startCol = vm.StartColumn
                });
            }

            try
            {
                var orientation = vm.Orientation!.Value;

                bool canPosition = await _battleshipService.CanPositionShipAsync(
                    vm.GameId,
                    currentUser.Id,
                    vm.ShipType,
                    vm.StartRow,
                    vm.StartColumn,
                    orientation);

                if (!canPosition)
                {
                    TempData["ErrorMessage"] = "Debe cambiar la celda seleccionada o la direccion, ya que con la combinacion actual el barco quedaria posicionado encima de otro barco o fuera del tablero.";
                    return RedirectToAction("SelectDirection", new
                    {
                        gameId = vm.GameId,
                        shipType = (int)vm.ShipType,
                        startRow = vm.StartRow,
                        startCol = vm.StartColumn
                    });
                }

                // Posicionar el barco
                await _battleshipService.PositionShipAsync(
                    vm.GameId,
                    currentUser.Id,
                    vm.ShipType,
                    vm.StartRow,
                    vm.StartColumn,
                    orientation);

                // Redirigir de vuelta a seleccion de barcos
                return RedirectToAction("SelectShip", new { gameId = vm.GameId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("SelectDirection", new
                {
                    gameId = vm.GameId,
                    shipType = (int)vm.ShipType,
                    startRow = vm.StartRow,
                    startCol = vm.StartColumn
                });
            }
        }

        public async Task<IActionResult> WaitingForOpponent(int gameId)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login");

            var gameDto = await _battleshipService.GetGameByIdAsync(gameId);
            if (gameDto == null)
            {
                TempData["ErrorMessage"] = "La partida no existe.";
                return RedirectToAction("Index");
            }

            // Obtener barcos posicionados del usuario para mostrar el tablero
            var userShips = await _battleshipService.GetUserShipsAsync(gameId, currentUser.Id);

            var board = _boardBuilder.BuildShipBoard(userShips);

            foreach (var ship in userShips)
            {
                var cells = _boardBuilder.GetShipCells(ship.Type, ship.StartRow, ship.StartColumn, ship.Orientation);
                foreach (var (row, col) in cells)
                {
                    if (row >= 0 && row < 12 && col >= 0 && col < 12)
                    {
                        board[row, col] = ShipPositionCellState.Ship;
                    }
                }
            }

            var viewModel = new MyShipsBoardViewModel
            {
                GameId = gameId,
                Board = board
            };

            // Verificar si el oponente ya termino
            string opponentId = gameDto.CreatorUserId == currentUser.Id
                ? gameDto.OpponentUserId
                : gameDto.CreatorUserId;

            bool opponentReady = await _battleshipService.IsSetupCompleteAsync(gameId, opponentId);

            ViewBag.OpponentReady = opponentReady;
            ViewBag.Message = opponentReady
                ? "El otro jugador ya termino de configurar sus barcos. Puedes comenzar a atacar."
                : "El otro jugador aun no termina de configurar sus barcos.";

            return View(viewModel);
        }

        // ============================================
        // Tablero de ataque
        // ============================================

        public async Task<IActionResult> AttackBoard(int gameId)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login");

            var gameDto = await _battleshipService.GetGameByIdAsync(gameId);
            if (gameDto == null)
            {
                TempData["ErrorMessage"] = "La partida no existe.";
                return RedirectToAction("Index");
            }

            // Verificar si la partida termino
            if (gameDto.Status == BattleshipGameStatus.Finished)
            {
                TempData["InfoMessage"] = "Esta partida ya finalizo.";
                return RedirectToAction("Index");
            }

            // Obtener ataques del usuario
            var myAttacks = await _battleshipService.GetUserAttacksAsync(gameId, currentUser.Id);

            var board = _boardBuilder.BuildAttackBoard(myAttacks);

            // Marcar ataques 
            foreach (var attack in myAttacks)
            {
                if (attack.TargetRow >= 0 && attack.TargetRow < 12 &&
                    attack.TargetColumn >= 0 && attack.TargetColumn < 12)
                {
                    board[attack.TargetRow, attack.TargetColumn] = attack.Result == AttackResult.Hit || attack.Result == AttackResult.Sunk
                        ? AttackCellState.Hit
                        : AttackCellState.Miss;
                }
            }

            // Verificar de quien es el turno
            bool isMyTurn = await _battleshipService.IsUserTurnAsync(gameId, currentUser.Id);

            string opponentId = gameDto.CreatorUserId == currentUser.Id
                ? gameDto.OpponentUserId
                : gameDto.CreatorUserId;

            var opponent = await _accountService.GetUserById(opponentId);
            string currentTurnPlayerName = isMyTurn ? "Yo" : (opponent?.UserName ?? "Oponente");

            var turnNumber = myAttacks.Count + 1;

            var viewModel = new AttackBoardViewModel
            {
                GameId = gameId,
                Board = board,
                IsMyTurn = isMyTurn,
                CurrentTurnPlayerName = currentTurnPlayerName,
                TurnNumber = turnNumber
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Attack(AttackViewModel vm)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login");

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Datos de ataque invalidos.";
                return RedirectToAction("AttackBoard", new { gameId = vm.GameId });
            }

            try
            {
                var attackDto = await _battleshipService.AttackAsync(
                    vm.GameId,
                    currentUser.Id,
                    vm.TargetRow,
                    vm.TargetColumn);

                if (attackDto != null)
                {
                    string resultMessage = attackDto.Result == AttackResult.Hit
                        ? "Has impactado un barco enemigo."
                        : attackDto.Result == AttackResult.Sunk
                            ? "Has destruido un barco enemigo completamente."
                            : "Fallaste, no hay ningun barco en esa posicion.";

                    TempData["AttackResult"] = resultMessage;
                }

                return RedirectToAction("AttackBoard", new { gameId = vm.GameId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("AttackBoard", new { gameId = vm.GameId });
            }
        }

        public async Task<IActionResult> ViewMyShips(int gameId)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login");

            var gameDto = await _battleshipService.GetGameByIdAsync(gameId);
            if (gameDto == null)
            {
                TempData["ErrorMessage"] = "La partida no existe.";
                return RedirectToAction("Index");
            }

            // Obtener barcos del usuario
            var userShips = await _battleshipService.GetUserShipsAsync(gameId, currentUser.Id);
            var board = _boardBuilder.BuildShipBoard(userShips);

            var viewModel = new MyShipsBoardViewModel
            {
                GameId = gameId,
                Board = board
            };

            ViewBag.CanReturn = true;
            return View(viewModel);
        }

        // ============================================
        // Historial Y Resultados
        // ============================================

        public async Task<IActionResult> ViewResult(int gameId)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login");

            var viewModel = await BattleshipMappers.BuildGameResultAsync(
                gameId,
                currentUser.Id,
                _battleshipService,
                _accountService,
                _boardBuilder);

            if (viewModel == null)
            {
                TempData["ErrorMessage"] = "No se pudo cargar el resultado de la partida.";
                return RedirectToAction("Index");
            }

            ViewBag.CurrentView = "MyAttacks";
            return View(viewModel);
        }

        public async Task<IActionResult> ViewOpponentAttacks(int gameId)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login");

            var viewModel = await BattleshipMappers.BuildGameResultAsync(
                gameId,
                currentUser.Id,
                _battleshipService,
                _accountService,
                _boardBuilder);

            if (viewModel == null)
            {
                TempData["ErrorMessage"] = "No se pudo cargar el resultado de la partida.";
                return RedirectToAction("Index");
            }

            ViewBag.CurrentView = "OpponentAttacks";
            return View("ViewResult", viewModel);
        }

        public async Task<IActionResult> ViewMyShipsResult(int gameId)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Login");

            var viewModel = await BattleshipMappers.BuildGameResultAsync(
                gameId,
                currentUser.Id,
                _battleshipService,
                _accountService,
                _boardBuilder);

            if (viewModel == null)
            {
                TempData["ErrorMessage"] = "No se pudo cargar el resultado de la partida.";
                return RedirectToAction("Index");
            }

            ViewBag.CurrentView = "MyShips";
            return View("ViewResult", viewModel);
        }
    }
}