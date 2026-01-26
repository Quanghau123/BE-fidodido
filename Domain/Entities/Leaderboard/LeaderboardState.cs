using FidoDino.Domain.Enums;
using FidoDino.Domain.Entities.Auth;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Domain.Entities.Leaderboard
{
    public class LeaderboardState
    {
        public Guid LeaderboardStateId { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public TimeRangeType TimeRange { get; set; }
        public string TimeKey { get; set; } = null!;
        public int TotalScore { get; set; }
        public int PlayCount { get; set; }
        public DateTime AchievedAt { get; set; }
        public string StableRandom { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
    }
}