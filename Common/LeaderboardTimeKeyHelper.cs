using System;
using System.Globalization;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Common
{
    public static class LeaderboardTimeKeyHelper
    {
        public static DateTime ParseWeekKey(string weekKey)
        {
            // weekKey: "2026-W04"
            var parts = weekKey.Split('-');
            int year = int.Parse(parts[0]);
            int week = int.Parse(parts[1].Substring(1));
            // Lấy ngày đầu tuần (thứ 2)
            var jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
            var firstMonday = jan1.AddDays(daysOffset);
            var result = firstMonday.AddDays((week - 1) * 7);
            return result;
        }

        public static DateTime ParseMonthKey(string monthKey)
        {
            // monthKey: "2026-01"
            var parts = monthKey.Split('-');
            int year = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);
            return new DateTime(year, month, 1);
        }
        
        public static string GetDayKey(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static string GetWeekKey(DateTime date)
        {
            int isoWeek = ISOWeek.GetWeekOfYear(date);
            return $"{date:yyyy}-W{isoWeek:D2}";
        }

        public static string GetMonthKey(DateTime date)
        {
            return date.ToString("yyyy-MM");
        }

        public static string GetTimeKey(TimeRangeType timeRange, DateTime date)
        {
            switch (timeRange)
            {
                case TimeRangeType.Day:
                    return GetDayKey(date);
                case TimeRangeType.Week:
                    return GetWeekKey(date);
                case TimeRangeType.Month:
                    return GetMonthKey(date);
                default:
                    throw new ArgumentException("Invalid timeRange", nameof(timeRange));
            }
        }
    }
}
