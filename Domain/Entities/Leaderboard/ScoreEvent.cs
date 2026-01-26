using FidoDino.Domain.Enums.Game;

namespace FidoDino.Domain.Entities.Leaderboard
{
    public class ScoreEvent
    {
        public Guid ScoreEventId { get; set; }
        public Guid UserId { get; set; }
        public Auth.User User { get; set; } = null!;
        public Guid GameSessionId { get; set; }
        public Game.GameSession GameSession { get; set; } = null!;
        public int ScoreDelta { get; set; }
        public long? CompositeDelta { get; set; } 
        public TimeRangeType TimeRange { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool AppliedToRedis { get; set; }
    }
}