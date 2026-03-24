using LinkUpApp.Core.Application.Dtos.Social;
using LinkUpApp.Core.Application.Interfaces.Base;

namespace LinkUpApp.Core.Application.Interfaces.Social
{
    public interface ICommentService : IGenericService<CommentDto>
    {
        Task<List<CommentDto>> GetCommentsByPostIdAsync(int postId);
        Task<List<CommentDto>> GetRepliesByCommentIdAsync(int commentId);
        Task<List<CommentDto>> GetCommentsWithRepliesAsync(int postId);
    }
}
