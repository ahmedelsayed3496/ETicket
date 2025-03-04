using ETicket.Repositories.IRepositories;
using ETicket.Data;
using ETicket.Models;

namespace ETicket.Repositories
{
    public class CinemaRepository : Repository<Cinema>, ICinemaRepository
    {
        private readonly ApplicationDbContext dbContext;

        public CinemaRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }


    }
}
