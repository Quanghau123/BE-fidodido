using FidoDino.Domain.Entities.Auth;

namespace FidoDino.Domain.Entities.Game
{
    public class GameSession
    {
        public Guid GameSessionId { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; }
        public int TotalScore { get; set; }
        public ICollection<PlayTurn> PlayTurns { get; set; } = new List<PlayTurn>();
        public ICollection<ActiveEffect> ActiveEffects { get; set; } = new List<ActiveEffect>();
    }
}