namespace FidoDino.Application.DTOs.Game
{
    public class CompletePlayTurnResultDto
    {
        public Guid PlayTurnId { get; set; }
        public int EarnedScore { get; set; }
    }
}