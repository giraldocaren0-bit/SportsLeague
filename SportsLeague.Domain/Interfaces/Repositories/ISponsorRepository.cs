
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ISponsorRepository: IGenericRepository<Sponsor>
    {
        Task<Sponsor?>ExistsbyNameAsync(string name);
        Task<Sponsor?> ExistsbyContactEmailAsync(string contactEmail);

        Task<IEnumerable<Sponsor>> GetbyCategoryAsync(SponsorCategory category);

    }
}
