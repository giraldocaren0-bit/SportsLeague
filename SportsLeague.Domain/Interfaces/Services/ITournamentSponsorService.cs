using SportsLeague.Domain.Entities;


namespace SportsLeague.Domain.Interfaces.Services
{
    public interface ITournamentSponsorService
    {
        Task<TournamentSponsor> AddSponsorToTournamentAsync(int tournamentId, int sponsorId, decimal contractAmount);
        Task DeleteSponsorFromTournamentAsync(int tournamentId, int sponsorId);
        Task<IEnumerable<TournamentSponsor>> GetSponsorsByTournamentIdAsync(int tournamentId);
        Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorIdAsync(int sponsorId);
    }
}
