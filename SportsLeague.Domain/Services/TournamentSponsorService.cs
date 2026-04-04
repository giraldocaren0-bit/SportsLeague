using SportsLeague.Domain.Interfaces.Services;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace SportsLeague.Domain.Services
{
    public class TournamentSponsorService : ITournamentSponsorService
    {
        private readonly ILogger _logger;
        private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ITournamentRepository _tournamentRepository;

        public TournamentSponsorService(ITournamentSponsorRepository tournamentSponsorRepository, ILogger<TournamentSponsorService> logger, ISponsorRepository sponsorRepository, ITournamentRepository tournamentRepository)
        {
            _tournamentSponsorRepository = tournamentSponsorRepository;
            _logger = logger;
            _sponsorRepository = sponsorRepository;
            _tournamentRepository = tournamentRepository;
        }

        public async Task<TournamentSponsor> AddSponsorToTournamentAsync(int tournamentId, int sponsorId, decimal contractAmount)
        {
            var sponsor = await _tournamentSponsorRepository.GetBySponsorIdAsync(sponsorId); if (sponsor == null)

                throw new KeyNotFoundException($"No se encontró el patrocinador con ID {sponsorId}");

            var tournament = await _tournamentSponsorRepository.GetSByTournamentIdAsync(tournamentId); if (tournament == null)

                throw new KeyNotFoundException($"No se encontró el torneo con ID {tournamentId}");

            if (contractAmount <= 0)

            {

                throw new InvalidOperationException("El monto debe ser mayor a 0");

            }

            var existing = await _tournamentSponsorRepository

                .GetByTournamentIdAndSponsorIdAsync(tournamentId, sponsorId); if (existing != null)

            {

                throw new InvalidOperationException("Este patrocinador ya está en este torneo");

            }

            var tournamentSponsor = new TournamentSponsor
            {

                SponsorId = sponsorId,

                TournamentId = tournamentId,

                ContractAmount = contractAmount,

                JoinedAt = DateTime.UtcNow

            };


            _logger.LogInformation("Registering sponsor {SponsorId} to tournament {TournamentId} with amount {ContractAmount}",

                sponsorId, tournamentId, contractAmount);

            return await _tournamentSponsorRepository.CreateAsync(tournamentSponsor);

        }

    public async Task DeleteSponsorFromTournamentAsync(int tournamentId, int sponsorId)
        {
            var existing = await _tournamentSponsorRepository.GetByTournamentIdAndSponsorIdAsync(tournamentId, sponsorId);
            if (existing == null)
            {
                throw new KeyNotFoundException("No se encontró la relación entre el patrocinador y el torneo");
            }
            _logger.LogInformation("Deleting sponsor {SponsorId} from tournament {TournamentId}", sponsorId, tournamentId);
            await _tournamentSponsorRepository.DeleteAsync(existing.Id);
        }

        public async Task<IEnumerable<TournamentSponsor>> GetSponsorsByTournamentIdAsync(int tournamentId)
        {
            try {
                var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
                if (tournament == null)
                {
                    throw new KeyNotFoundException($"No se encontró el torneo con ID {tournamentId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el torneo con ID {TournamentId}", tournamentId);
                throw;
            }
            return await _tournamentSponsorRepository.GetSByTournamentIdAsync(tournamentId);
        }
        public async Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorIdAsync(int sponsorId)
        {
            try
            {
                var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);
                if (sponsor == null)
                {
                    throw new KeyNotFoundException($"No se encontró el patrocinador con ID {sponsorId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el patrocinador con ID {SponsorId}", sponsorId);
                throw;
            }
            return await _tournamentSponsorRepository.GetBySponsorIdAsync(sponsorId);
        }
    }
}