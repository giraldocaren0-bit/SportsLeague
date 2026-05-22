using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Helpers;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services;

public class MatchLineupService : IMatchLineupService
{
    private readonly IMatchLineupRepository _matchLineupRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly MatchValidationHelper _validationHelper;
    private readonly ILogger<MatchLineupService> _logger;

    public MatchLineupService(
        IMatchLineupRepository matchLineupRepository,
        IMatchRepository matchRepository,
        IPlayerRepository playerRepository,
        MatchValidationHelper validationHelper,
        ILogger<MatchLineupService> logger)
    {
        _matchLineupRepository = matchLineupRepository;
        _matchRepository = matchRepository;
        _playerRepository = playerRepository;
        _validationHelper = validationHelper;
        _logger = logger;
    }

    public async Task<MatchLineup> AddToLineupAsync(int matchId, MatchLineup lineup)
    {
        // V1: El partido debe existir y V6: Solo Scheduled
        var match = await _validationHelper.ValidateMatchForLineupAsync(matchId);
        if (match.Status != MatchStatus.Scheduled)
            throw new InvalidOperationException("Solo se pueden registrar alineaciones en partidos Scheduled");

        // V2: El jugador debe existir
        var player = await _playerRepository.GetByIdAsync(lineup.PlayerId);
        if (player == null)
            throw new KeyNotFoundException($"No se encontró el jugador con ID {lineup.PlayerId}");

        // V3: El jugador debe pertenecer al HomeTeam o AwayTeam
        if (player.TeamId != match.HomeTeamId && player.TeamId != match.AwayTeamId)
            throw new InvalidOperationException("El jugador no pertenece a ninguno de los equipos del partido");

        // V4: El jugador no puede estar registrado dos veces
        var exists = await _matchLineupRepository.ExistsByMatchAndPlayerAsync(matchId, lineup.PlayerId);
        if (exists)
            throw new InvalidOperationException("El jugador ya está registrado en la alineación de este partido");

        // V5: Máximo 11 titulares por equipo
        if (lineup.IsStarter)
        {
            var startersCount = await _matchLineupRepository.CountStartersByMatchAndTeamAsync(matchId, player.TeamId);
            if (startersCount >= 11)
                throw new InvalidOperationException("El equipo ya tiene 11 titulares registrados en este partido");
        }

        lineup.MatchId = matchId;
        _logger.LogInformation("Adding player {PlayerId} to lineup for match {MatchId}", lineup.PlayerId, matchId);
        return await _matchLineupRepository.CreateAsync(lineup);
    }

    public async Task<IEnumerable<MatchLineup>> GetLineupByMatchAsync(int matchId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

        return await _matchLineupRepository.GetByMatchAsync(matchId);
    }

    public async Task<IEnumerable<MatchLineup>> GetLineupByMatchAndTeamAsync(int matchId, int teamId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

        return await _matchLineupRepository.GetByMatchAndTeamAsync(matchId, teamId);
    }

    public async Task RemoveFromLineupAsync(int id)
    {
        var exists = await _matchLineupRepository.ExistsAsync(id);
        if (!exists)
            throw new KeyNotFoundException($"No se encontró la alineación con ID {id}");

        await _matchLineupRepository.DeleteAsync(id);
        _logger.LogInformation("Removed lineup record {LineupId}", id);
    }
}