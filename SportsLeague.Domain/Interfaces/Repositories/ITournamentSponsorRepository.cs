using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ITournamentSponsorRepository : IGenericRepository<TournamentSponsor>
    {
            Task<IEnumerable<TournamentSponsor>> GetSByTournamentIdAsync(int tournamentId); //metodo para obtener los patrocinadores de un torneo
        Task<IEnumerable<TournamentSponsor>> GetBySponsorIdAsync(int sponsorId); //metodo para obtener los torneos de un patrocinador
        Task<TournamentSponsor?> GetByTournamentIdAndSponsorIdAsync(int tournamentId, int sponsorId); //metodo para obtener la relacion entre un torneo y un patrocinador
    }
}
