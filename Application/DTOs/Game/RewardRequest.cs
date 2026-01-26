namespace FidoDino.Application.DTOs.Game
{
    public class RewardRequestAdd
    {
        public string RewardName { get; set; } = string.Empty;
        public int Score { get; set; }
        public Guid? EffectId { get; set; }
    }
    public class RewardRequestUpdate
    {
        public Guid RewardId { get; set; }
        public string RewardName { get; set; } = string.Empty;
        public int Score { get; set; }
        public Guid? EffectId { get; set; }
    }
}
