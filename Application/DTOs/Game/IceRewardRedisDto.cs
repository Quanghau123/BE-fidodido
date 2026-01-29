namespace FidoDino.Application.DTOs.Game
{
    public class IceRewardRedisDto
    {
        public Guid IceId { get; set; }
        public Guid RewardId { get; set; }
        public double Probability { get; set; }
    }
}