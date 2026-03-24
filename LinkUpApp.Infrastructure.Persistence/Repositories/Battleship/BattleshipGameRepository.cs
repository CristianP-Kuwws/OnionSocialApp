using LinkUpApp.Core.Domain.Entities.Battleship;
using LinkUpApp.Core.Domain.Interfaces.Battleship;
using LinkUpApp.Infrastructure.Persistence.Contexts;
using LinkUpApp.Infrastructure.Persistence.Repositories.Base;

namespace LinkUpApp.Infrastructure.Persistence.Repositories.Battleship
{
    public class BattleshipGameRepository : GenericRepository<BattleshipGame>, IBattleshipGameRepository
    {
        public BattleshipGameRepository(LinkUpAppContext context) : base(context)
        {

        }
    }
}
