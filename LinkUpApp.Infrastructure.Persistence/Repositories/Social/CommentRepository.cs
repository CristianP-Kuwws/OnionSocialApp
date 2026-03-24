using LinkUpApp.Core.Domain.Entities.Social;
using LinkUpApp.Core.Domain.Interfaces.Social;
using LinkUpApp.Infrastructure.Persistence.Contexts;
using LinkUpApp.Infrastructure.Persistence.Repositories.Base;

namespace LinkUpApp.Infrastructure.Persistence.Repositories.Social
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(LinkUpAppContext context) : base(context)
        {

        }
    }
}
