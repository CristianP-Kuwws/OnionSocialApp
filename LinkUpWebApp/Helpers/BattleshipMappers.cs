using LinkUpApp.Core.Application.Dtos.Battleship;
using LinkUpApp.Core.Application.Interfaces.Battleship;
using LinkUpApp.Core.Application.Interfaces.Helpers;
using LinkUpApp.Core.Application.Interfaces.User;
using LinkUpApp.Core.Application.ViewModels.Battleship.BattleshipGame;
using LinkUpApp.Core.Application.ViewModels.Battleship.Results;
using LinkUpApp.Core.Domain.Common.Enum.Battleship;

namespace LinkUpWebApp.Helpers
{
    public static class BattleshipMappers
    {
        public static async Task<List<ActiveGameViewModel>> MapActiveGamesAsync(
            List<BattleshipGameDto> gamesDto,
            string currentUserId,
            IAccountServicesForWebApp accountService)
        {
            var viewModels = new List<ActiveGameViewModel>();

            foreach (var game in gamesDto)
            {
                string opponentId = game.CreatorUserId == currentUserId
                    ? game.OpponentUserId
                    : game.CreatorUserId;

                var opponent = await accountService.GetUserById(opponentId);
                if (opponent == null) continue;

                var hoursElapsed = (int)(DateTime.UtcNow - game.CreatedAt).TotalHours;

                var mySetupStatus = game.CreatorUserId == currentUserId
                    ? game.CreatorSetupStatus
                    : game.OpponentSetupStatus;

                var opponentSetupStatus = game.CreatorUserId == currentUserId
                    ? game.OpponentSetupStatus
                    : game.CreatorSetupStatus;

                viewModels.Add(new ActiveGameViewModel
                {
                    GameId = game.Id,
                    OpponentUserName = opponent.UserName,
                    OpponentFullName = $"{opponent.FirstName} {opponent.LastName}",
                    OpponentProfilePicture = opponent.ProfilePicturePath,
                    CreatedAt = game.CreatedAt,
                    HoursElapsed = hoursElapsed,
                    MySetupStatus = mySetupStatus,
                    OpponentSetupStatus = opponentSetupStatus
                });
            }

            return viewModels;
        }

        public static async Task<List<GameHistoryViewModel>> MapGameHistoryAsync(
            List<BattleshipGameDto> gamesDto,
            string currentUserId,
            IAccountServicesForWebApp accountService)
        {
            var viewModels = new List<GameHistoryViewModel>();

            foreach (var game in gamesDto)
            {
                string opponentId = game.CreatorUserId == currentUserId
                    ? game.OpponentUserId
                    : game.CreatorUserId;

                var opponent = await accountService.GetUserById(opponentId);
                if (opponent == null) continue;

                bool didIWin = game.WinnerId == currentUserId;
                string winnerDisplay = didIWin ? "Yo" : opponent.UserName;

                var duration = game.FinishedAt.HasValue
                    ? (int)(game.FinishedAt.Value - game.CreatedAt).TotalHours
                    : 0;

                viewModels.Add(new GameHistoryViewModel
                {
                    GameId = game.Id,
                    OpponentUserName = opponent.UserName,
                    OpponentFullName = $"{opponent.FirstName} {opponent.LastName}",
                    OpponentProfilePicture = opponent.ProfilePicturePath,
                    CreatedAt = game.CreatedAt,
                    FinishedAt = game.FinishedAt ?? DateTime.UtcNow,
                    DurationInHours = duration,
                    DidIWin = didIWin,
                    WinnerDisplay = winnerDisplay
                });
            }

            return viewModels;
        }

        public static async Task<GameResultViewModel?> BuildGameResultAsync(
            int gameId,
            string currentUserId,
            IBattleshipService battleshipService,
            IAccountServicesForWebApp accountService,
            IBoardBuilder boardBuilder)
        {
            var gameDto = await battleshipService.GetGameByIdAsync(gameId);
            if (gameDto == null || gameDto.Status != BattleshipGameStatus.Finished)
                return null;

            string opponentId = gameDto.CreatorUserId == currentUserId
                ? gameDto.OpponentUserId
                : gameDto.CreatorUserId;

            var opponent = await accountService.GetUserById(opponentId);

            // Obtener mis ataques
            var myAttacks = await battleshipService.GetUserAttacksAsync(gameId, currentUserId);
            var myAttackBoard = boardBuilder.BuildAttackBoard(myAttacks);

            // Obtener ataques del oponente
            var opponentAttacks = await battleshipService.GetUserAttacksAsync(gameId, opponentId);
            var opponentAttackBoard = boardBuilder.BuildAttackBoard(opponentAttacks);

            // Obtener mis barcos
            var myShips = await battleshipService.GetUserShipsAsync(gameId, currentUserId);
            var myShipBoard = boardBuilder.BuildShipBoardWithHits(myShips, opponentAttacks);

            bool didIWin = gameDto.WinnerId == currentUserId;
            var duration = gameDto.FinishedAt.HasValue
                ? (int)(gameDto.FinishedAt.Value - gameDto.StartedAt.GetValueOrDefault(gameDto.CreatedAt)).TotalHours
                : 0;

            return new GameResultViewModel
            {
                GameId = gameId,
                OpponentUserName = opponent?.UserName ?? "Oponente",
                DidIWin = didIWin,
                MyAttackBoard = myAttackBoard,
                OpponentAttackBoard = opponentAttackBoard,
                MyShipBoard = myShipBoard,
                StartedAt = gameDto.StartedAt ?? gameDto.CreatedAt,
                FinishedAt = gameDto.FinishedAt ?? DateTime.UtcNow,
                DurationInHours = duration
            };
        }
    }
}
