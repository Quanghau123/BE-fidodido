using FidoDino.Domain.Enums.Game;

namespace FidoDino.Domain.Entities.Game
{
    public class PlayTurn
    {
        public Guid PlayTurnId { get; set; }
        public Guid GameSessionId { get; set; }
        public GameSession GameSession { get; set; } = null!;
        public Guid IceId { get; set; }
        public Ice Ice { get; set; } = null!;
        public Guid RewardId { get; set; }
        public Reward Reward { get; set; } = null!;
        public int ShakeCount { get; set; }
        public int EarnedScore { get; set; }
        public DateTime PlayedAt { get; set; }
        public TimeRangeType TimeRange { get; set; }
        public string TimeKey { get; set; } = null!;
    }
}