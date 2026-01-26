namespace FidoDino.Application.DTOs.Game
{
    public class RewardResultDto
    {
        public Guid RewardId { get; set; }
        public string RewardName { get; set; } = string.Empty;
        public int Score { get; set; }
        public string? EffectType { get; set; }
    }
}
