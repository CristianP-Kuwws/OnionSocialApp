using AutoMapper;
using LinkUpApp.Core.Application.Dtos.Social;
using LinkUpApp.Core.Application.Interfaces.Social;
using LinkUpApp.Core.Application.Services.Base;
using LinkUpApp.Core.Domain.Entities.Social;
using LinkUpApp.Core.Domain.Interfaces.Social;
using Microsoft.EntityFrameworkCore;

namespace LinkUpApp.Core.Application.Services.Social
{
    public class PostService : GenericService<Post, PostDto>, IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IMapper _mapper;

        public PostService(
            IPostRepository postRepository,
            IFriendshipRepository friendshipRepository,
            IMapper mapper) : base(postRepository, mapper)
        {
            _postRepository = postRepository;
            _friendshipRepository = friendshipRepository;
            _mapper = mapper;
        }

        public async Task<List<PostDto>> GetPostsByUserIdAsync(string userId)
        {
            try
            {
                var query = _postRepository.GetAllQueryWithInclude(new List<string>
                {
                    "Comments",
                    "Reactions"
                });

                var posts = await query
                    .Where(p => p.UserId == userId)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return _mapper.Map<List<PostDto>>(posts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetPostsByUserIdAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<PostDto>> GetFriendsPostsAsync(string userId)
        {
            try
            {
                var friendships = await _friendshipRepository.GetAllListAsync();
                var friendIds = friendships
                    .Where(f => f.User1Id == userId || f.User2Id == userId)
                    .Select(f => f.User1Id == userId ? f.User2Id : f.User1Id)
                    .ToList();

                if (!friendIds.Any())
                    return new List<PostDto>();

                var query = _postRepository.GetAllQueryWithInclude(new List<string>
                {
                    "Comments",
                    "Reactions"
                });

                var posts = await query
                    .Where(p => friendIds.Contains(p.UserId))
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return _mapper.Map<List<PostDto>>(posts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetFriendsPostsAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<PostDto?> GetPostWithDetailsAsync(int postId)
        {
            try
            {
                var query = _postRepository.GetAllQueryWithInclude(new List<string>
                {
                    "Comments.Replies",
                    "Reactions"
                });

                var post = await query.FirstOrDefaultAsync(p => p.Id == postId);

                if (post == null)
                    return null;

                return _mapper.Map<PostDto>(post);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetPostWithDetailsAsync: {ex.Message}");
                throw;
            }
        }
    }

}
