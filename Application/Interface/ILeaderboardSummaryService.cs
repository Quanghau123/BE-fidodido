using FidoDino.Domain.Enums.Game;

namespace FidoDino.Application.Interfaces
{
    public interface ILeaderboardSummaryService
    {
        Task SummarizeAndResetAsync(TimeRangeType timeRange, int topN);
    }
}