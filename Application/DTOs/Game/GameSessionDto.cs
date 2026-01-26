namespace FidoDino.Application.DTOs.Game
{
    public class GameSessionDto
    {
        public Guid GameSessionId { get; set; }
        public Guid UserId { get; set; }
        public int TotalScore { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; }
    }
}