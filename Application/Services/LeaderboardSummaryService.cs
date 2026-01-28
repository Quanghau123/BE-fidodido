using FidoDino.Domain.Enums.Game;
using FidoDino.Domain.Entities.Leaderboard;
using FidoDino.Domain.Interfaces.Leaderboard;
using FidoDino.Common;
using FidoDino.Application.Interfaces;
using FidoDino.Domain.Interfaces.Game;

namespace FidoDino.Application.Services
{
    public class LeaderboardSummaryService : ILeaderboardSummaryService
    {
        private readonly ILeaderboardRepository _leaderboardRepository;
        private readonly ILeaderboardSnapshotRepository _snapshotRepository;
        private readonly ILeaderboardStateRepository _leaderboardStateRepository;
        private readonly IGameSessionRepository _gameSessionRepository;
        private readonly IPlayTurnRepository _playTurnRepository;
        public LeaderboardSummaryService(
            ILeaderboardRepository leaderboardRepository,
            ILeaderboardSnapshotRepository snapshotRepository,
            ILeaderboardStateRepository leaderboardStateRepository,
            IGameSessionRepository gameSessionRepository,
            IPlayTurnRepository playTurnRepository)
        {
            _leaderboardRepository = leaderboardRepository;
            _snapshotRepository = snapshotRepository;
            _leaderboardStateRepository = leaderboardStateRepository;
            _gameSessionRepository = gameSessionRepository;
            _playTurnRepository = playTurnRepository;
        }

        public async Task SummarizeAndResetAsync(TimeRangeType timeRange, int topN)
        {
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var timeKey = LeaderboardTimeKeyHelper.GetTimeKey(timeRange, now);

            var topUsers = (await _leaderboardRepository
                .GetTopAsync(timeRange, now, topN))
                .ToList();

            for (int i = 0; i < topUsers.Count; i++)
            {
                var (userId, _, realScore) = topUsers[i];

                var snapshot = new LeaderboardSnapshot
                {
                    LeaderboardSnapshotId = Guid.NewGuid(),
                    TimeRange = timeRange,
                    TimeKey = timeKey,
                    UserId = userId,
                    Rank = i + 1,
                    TotalScore = realScore,
                    CreatedAt = DateTime.UtcNow
                };

                await _snapshotRepository.AddAsync(snapshot);
            }

            await _leaderboardStateRepository.DeleteByTimeRangeAsync(timeRange, timeKey);
            await _gameSessionRepository.DeleteByTimeRangeAsync(timeRange, timeKey);
            await _playTurnRepository.DeleteByTimeRangeAsync(timeRange, timeKey);

            await _leaderboardRepository.ResetAsync(timeRange, now);
        }
    }
}
