using System;

namespace FidoDino.Domain.Entities.Game
{
    public class GameSessionSnapshot
    {
        public Guid GameSessionSnapshotId { get; set; }
        public Guid GameSessionId { get; set; }
        public Guid UserId { get; set; }
        public int TotalScore { get; set; }
        public DateTime? LastPlayedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
