using FidoDino.Domain.Enums;
using FidoDino.Domain.Entities.Leaderboard;
using FidoDino.Domain.Enums.User;

namespace FidoDino.Domain.Entities.Auth
{
    public class User
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public AuthProvider AuthProvider { get; set; } = AuthProvider.Local;
        public string? ProviderUserId { get; set; }
        public bool IsActive { get; set; } = true;
        public UserRole UserRole { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Game.GameSession> GameSessions { get; set; } = new List<Game.GameSession>();
        public ICollection<LeaderboardState> LeaderboardStates { get; set; } = new List<LeaderboardState>();
    }
}