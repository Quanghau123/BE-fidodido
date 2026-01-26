using Microsoft.EntityFrameworkCore;
using FidoDino.Domain.Entities.Auth;
using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Entities.Leaderboard;
using FidoDino.Domain.Entities.System;
using FidoDino.Domain.Entities;

namespace FidoDino.Infrastructure.Data
{
    public class FidoDinoDbContext : DbContext
    {
        public FidoDinoDbContext(DbContextOptions<FidoDinoDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<GameSession> GameSessions { get; set; }
        public DbSet<PlayTurn> PlayTurns { get; set; }
        public DbSet<Ice> Ices { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<Effect> Effects { get; set; }
        public DbSet<ActiveEffect> ActiveEffects { get; set; }
        public DbSet<IceReward> IceRewards { get; set; }
        public DbSet<LeaderboardSnapshot> LeaderboardSnapshots { get; set; }
        public DbSet<GameSessionSnapshot> GameSessionSnapshots { get; set; }
        public DbSet<LeaderboardState> LeaderboardStates { get; set; }
        public DbSet<SystemStatus> SystemStatuses { get; set; }
        public DbSet<ScoreEvent> ScoreEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

// dotnet ef migrations add UpdateRefreshTokenEntity
// dotnet ef database update