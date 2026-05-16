using SportsLeague.Domain.Entities;


namespace SportsLeague.Domain.Interfaces.Services
{
    public interface ITournamentSponsorService
    {
        Task<TournamentSponsor> AddSponsorToTournamentAsync(int tournamentId, int sponsorId, decimal contractAmount);//agregar un patrocinador a un torneo con el monto del contrato
        Task DeleteSponsorFromTournamentAsync(int tournamentId, int sponsorId);
        Task<IEnumerable<TournamentSponsor>> GetSponsorsByTournamentIdAsync(int tournamentId);//obtener los patrocinadores asociados a un torneo
        Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorIdAsync(int sponsorId);
    }
}
