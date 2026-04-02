using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SportsLeague.Domain.Enums;


namespace SportsLeague.DataAccess.Repositories
{
    public class SponsorRepository: GenericRepository<Sponsor>, ISponsorRepository
    {
        public SponsorRepository(LeagueDbContext context) : base(context)
        {
        }
        public async Task<Sponsor?> ExistsbyContactEmailAsync(string contactEmail)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.ContactEmail == contactEmail);
        }
        public async Task<Sponsor?> ExistsbyNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.Name == name);
        }
        public async Task<IEnumerable<Sponsor>> GetbyCategoryAsync(SponsorCategory category)
        {
            return await _dbSet
                .Where(s => s.Category == category)
                .ToListAsync();
        }

    }
}
