namespace FidoDino.Domain.Entities.Game
{
    public class ActiveEffect
    {
        public Guid ActiveEffectId { get; set; }
        public Guid EffectId { get; set; }
        public Effect Effect { get; set; } = null!;
        public Guid GameSessionId { get; set; }
        public GameSession GameSession { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}