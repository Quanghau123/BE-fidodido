namespace FidoDino.Domain.Entities.Game
{
    public class IceReward
    {
        public Guid IceRewardId { get; set; }
        public Guid IceId { get; set; }
        public Ice Ice { get; set; } = null!;
        public Guid RewardId { get; set; }
        public Reward Reward { get; set; } = null!;
        public double Probability { get; set; }
    }
}