using LinkUpApp.Core.Application.Dtos.Social;
using LinkUpApp.Core.Application.Interfaces.Base;

namespace LinkUpApp.Core.Application.Interfaces.Social
{
    public interface IPostService : IGenericService<PostDto>
    {
        Task<List<PostDto>> GetPostsByUserIdAsync(string userId);
        Task<List<PostDto>> GetFriendsPostsAsync(string userId);
        Task<PostDto?> GetPostWithDetailsAsync(int postId);
    }
}
