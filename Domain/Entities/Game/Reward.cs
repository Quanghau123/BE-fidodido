namespace FidoDino.Domain.Entities.Game
{
    public class Reward
    {
        public Guid RewardId { get; set; }
        public string RewardName { get; set; } = null!;
        public int Score { get; set; }
        public Guid? EffectId { get; set; }
        public Effect Effect { get; set; } =  null!;
        public ICollection<IceReward> IceRewards { get; set; } = new List<IceReward>();
    }
}