using Microsoft.Extensions.Logging;

namespace FietsGame.Services;
public interface ILeaderboardService
{
    Task<List<Player>> GetPlayers();
    Task<Player> AddPlayer(Player player);

    Task ClearLeaderboard();

}

public class LeaderboardService : ILeaderboardService
{
    private readonly ILogger<LeaderboardService> _logger;
    private readonly ILeaderboardRepository _leaderboardRepository;

    public LeaderboardService(ILogger<LeaderboardService> logger, ILeaderboardRepository leaderboardRepository)
    {
        _logger = logger;
        _leaderboardRepository = leaderboardRepository;
    }

    public async Task<List<Player>> GetPlayers() => await _leaderboardRepository.GetPlayers();

    public async Task<Player> AddPlayer(Player player) => await _leaderboardRepository.AddPlayer(player);

    public async Task ClearLeaderboard() => await _leaderboardRepository.ClearLeaderboard();
}