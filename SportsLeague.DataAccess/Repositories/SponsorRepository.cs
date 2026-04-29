
using global::SportsLeague.DataAccess.Context;
using global::SportsLeague.Domain.Entities.SportsLeague.Domain.Entities;
using global::SportsLeague.Domain.Interfaces.Repositories.SportsLeague.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SportsLeague.DataAccess.Repositories
{
    public class SponsorRepository : GenericRepository<Sponsor>, ISponsorRepository
    {
        public SponsorRepository(LeagueDbContext context) : base(context)
        {
        }

        public async Task<Sponsor?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(
                sponsor => sponsor.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(
                sponsor => sponsor.Name.ToLower() == name.ToLower());
        }
    }
}

