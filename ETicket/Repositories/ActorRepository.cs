using ETicket.Repositories.IRepositories;
using ETicket.Data;
using ETicket.Models;

namespace ETicket.Repositories
{
    public class ActorRepository : Repository<Actor>, IActorRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ActorRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
