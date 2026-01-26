using FidoDino.Domain.Enums;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Domain.Entities.Leaderboard
{
    public class LeaderboardSnapshot
    {
        public Guid LeaderboardSnapshotId { get; set; }
        public TimeRangeType TimeRange { get; set; }
        public string TimeKey { get; set; } = null!;
        public Guid UserId { get; set; }
        public int Rank { get; set; }
        public int TotalScore { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}