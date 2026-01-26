namespace FidoDino.Application.DTOs.Game
{
    public class PlayTurnResultDto
    {
        public Guid PlayTurnId { get; set; }
        public Guid IceId { get; set; }
        public string IceName { get; set; } = string.Empty;
        public int ShakeCount { get; set; }
        public Guid RewardId { get; set; }
        public string RewardName { get; set; } = string.Empty;
        public int EarnedScore { get; set; }
        public string EffectInfo { get; set; } = string.Empty;
        public int EffectDuration { get; set; }
    }
}
