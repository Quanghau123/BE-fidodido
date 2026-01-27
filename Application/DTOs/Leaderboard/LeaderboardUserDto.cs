public class LeaderboardUserDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int Score { get; set; } 
    public long CompositeScore { get; set; } 
}