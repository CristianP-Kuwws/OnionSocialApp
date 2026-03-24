using AutoMapper;
using LinkUpApp.Core.Application.Dtos.Social;
using LinkUpApp.Core.Application.Interfaces.Social;
using LinkUpApp.Core.Application.Services.Base;
using LinkUpApp.Core.Domain.Entities.Social;
using LinkUpApp.Core.Domain.Interfaces.Social;
using Microsoft.EntityFrameworkCore;

namespace LinkUpApp.Core.Application.Services.Social
{
    public class CommentService : GenericService<Comment, CommentDto>, ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;

        public CommentService(
            ICommentRepository commentRepository,
            IMapper mapper) : base(commentRepository, mapper)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
        }
        public async Task<List<CommentDto>> GetCommentsByPostIdAsync(int postId)
        {
            try
            {
                var query = _commentRepository.GetAllQueryWithInclude(new List<string>
                {
                    "Replies"
                });

                var comments = await query
                    .Where(c => c.PostId == postId && c.ParentCommentId == null)
                    .OrderBy(c => c.CreatedAt)
                    .ToListAsync();

                return _mapper.Map<List<CommentDto>>(comments);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetCommentsByPostIdAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<CommentDto>> GetRepliesByCommentIdAsync(int commentId)
        {
            try
            {
                var query = _commentRepository.GetAllQuery();

                var replies = await query
                    .Where(c => c.ParentCommentId == commentId)
                    .OrderBy(c => c.CreatedAt)
                    .ToListAsync();

                return _mapper.Map<List<CommentDto>>(replies);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetRepliesByCommentIdAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<CommentDto>> GetCommentsWithRepliesAsync(int postId)
        {
            try
            {
                var query = _commentRepository.GetAllQueryWithInclude(new List<string>
                {
                    "Replies.Replies"
                });

                var comments = await query
                    .Where(c => c.PostId == postId && c.ParentCommentId == null)
                    .OrderBy(c => c.CreatedAt)
                    .ToListAsync();

                return _mapper.Map<List<CommentDto>>(comments);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetCommentsWithRepliesAsync: {ex.Message}");
                throw;
            }
        }

    }
}
