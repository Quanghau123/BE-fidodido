namespace FidoDino.Application.DTOs.Game
{
    public class RewardRedisDto
    {
        public Guid RewardId { get; set; }
        public Guid? EffectId { get; set; }
        public string RewardName { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}