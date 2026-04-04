using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ITournamentSponsorRepository : IGenericRepository<TournamentSponsor>
    {
            Task<IEnumerable<TournamentSponsor>> GetSByTournamentIdAsync(int tournamentId);
            Task<IEnumerable<TournamentSponsor>> GetBySponsorIdAsync(int sponsorId);
            Task<TournamentSponsor?> GetByTournamentIdAndSponsorIdAsync(int tournamentId, int sponsorId);
    }
}
