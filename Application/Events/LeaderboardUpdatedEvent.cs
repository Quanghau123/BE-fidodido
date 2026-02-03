using System;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Application.Events
{
    public class LeaderboardUpdatedEvent
    {
        public LeaderboardUpdatedEvent(TimeRangeType timeRange, DateTime date, int top)
        {
            TimeRange = timeRange;
            Date = date;
            Top = top;
        }
        public TimeRangeType TimeRange { get; }
        public DateTime Date { get; }
        public int Top { get; }
    }
}
