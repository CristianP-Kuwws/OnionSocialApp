using AutoMapper;
using LinkUpApp.Core.Application.Dtos.Social;
using LinkUpApp.Core.Application.Interfaces.Social;
using LinkUpApp.Core.Application.Services.Base;
using LinkUpApp.Core.Domain.Entities.Social;
using LinkUpApp.Core.Domain.Interfaces.Social;
using Microsoft.EntityFrameworkCore;

namespace LinkUpApp.Core.Application.Services.Social
{
    public class FriendshipService : GenericService<Friendship, FriendshipDto>, IFriendshipService
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IMapper _mapper;

        public FriendshipService(
            IFriendshipRepository friendshipRepository,
            IMapper mapper) : base(friendshipRepository, mapper)
        {
            _friendshipRepository = friendshipRepository;
            _mapper = mapper;
        }

        public async Task<List<FriendshipDto>> GetFriendsAsync(string userId)
        {
            try
            {
                var query = _friendshipRepository.GetAllQuery();

                var friendships = await query
                    .Where(f => f.User1Id == userId || f.User2Id == userId)
                    .ToListAsync();

                return _mapper.Map<List<FriendshipDto>>(friendships);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetFriendsAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<string>> GetFriendIdsAsync(string userId)
        {
            try
            {
                var query = _friendshipRepository.GetAllQuery();

                var friendships = await query
                    .Where(f => f.User1Id == userId || f.User2Id == userId)
                    .ToListAsync();

                // Retornar los id de amigos 
                var friendIds = friendships
                    .Select(f => f.User1Id == userId ? f.User2Id : f.User1Id)
                    .ToList();

                return friendIds;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetFriendIdsAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> RemoveFriendshipAsync(string userId, string friendId)
        {
            try
            {

                var user1Id = string.Compare(userId, friendId) < 0 ? userId : friendId;
                var user2Id = string.Compare(userId, friendId) < 0 ? friendId : userId;

                var query = _friendshipRepository.GetAllQuery();
                var friendship = await query
                    .FirstOrDefaultAsync(f => f.User1Id == user1Id && f.User2Id == user2Id);

                if (friendship == null)
                {
                    Console.WriteLine("Error: Amistad no encontrada");
                    return false;
                }

                await _friendshipRepository.DeleteAsync(friendship.Id);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en RemoveFriendshipAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<int> GetCommonFriendsCountAsync(string userId1, string userId2)
        {
            try
            {
                // Obtener amigos de userId1
                var friends1Ids = await GetFriendIdsAsync(userId1);

                // Obtener amigos de userId2
                var friends2Ids = await GetFriendIdsAsync(userId2);

                // Contar amigos en comun
                var commonFriendsCount = friends1Ids.Intersect(friends2Ids).Count();

                return commonFriendsCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetCommonFriendsCountAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> AreFriendsAsync(string userId1, string userId2)
        {
            try
            {
                var user1Id = string.Compare(userId1, userId2) < 0 ? userId1 : userId2;
                var user2Id = string.Compare(userId1, userId2) < 0 ? userId2 : userId1;

                var query = _friendshipRepository.GetAllQuery();
                var areFriends = await query
                    .AnyAsync(f => f.User1Id == user1Id && f.User2Id == user2Id);

                return areFriends;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR en AreFriendsAsync: {ex.Message}");
                throw;
            }
        }
    }
}

