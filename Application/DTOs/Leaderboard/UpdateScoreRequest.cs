using FidoDino.Domain.Entities.Leaderboard;

public class UpdateScoreRequest
{
    public LeaderboardState State { get; set; } = null!;
    public string TimeRange { get; set; } = null!;
    public string TimeKey { get; set; } = null!;
}