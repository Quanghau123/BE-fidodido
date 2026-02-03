namespace FidoDino.Application.DTOs.Game;

public class RewardEffectResultDto
{
    public string EffectType { get; set; }
    public int DurationSeconds { get; set; }

    public RewardEffectResultDto()
    {
        EffectType = Domain.Enums.Game.EffectType.None.ToString();
        DurationSeconds = 0;
    }
}
