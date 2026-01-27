using FidoDino.Domain.Enums.Game;
using FidoDino.Domain.Entities.Leaderboard;
using FidoDino.Domain.Interfaces.Leaderboard;
using FidoDino.Common;
using FidoDino.Application.Interfaces;

namespace FidoDino.Application.Services
{
    public class LeaderboardSummaryService : ILeaderboardSummaryService
    {
        private readonly ILeaderboardRepository _leaderboardRepository;
        private readonly ILeaderboardSnapshotRepository _snapshotRepository;

        public LeaderboardSummaryService(
            ILeaderboardRepository leaderboardRepository,
            ILeaderboardSnapshotRepository snapshotRepository)
        {
            _leaderboardRepository = leaderboardRepository;
            _snapshotRepository = snapshotRepository;
        }

        public async Task SummarizeAndResetAsync(TimeRangeType timeRange, int topN)
        {
            var now = DateTime.UtcNow;
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
                    CreatedAt = now
                };

                await _snapshotRepository.AddAsync(snapshot);
            }

            await _leaderboardRepository.ResetAsync(timeRange, now);
        }
    }
}
