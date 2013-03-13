
namespace Stocks.Common
{
    using System;

    public static class DateHelpers
    {
        public static bool IsTradingHour(this DateTime value)
        { 
            var currentTime = DateTime.Now;
            return (
                (currentTime.DayOfWeek != DayOfWeek.Sunday && currentTime.DayOfWeek != DayOfWeek.Saturday)
                &&
                (currentTime > new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 9, 34, 0)) 
                && 
                (currentTime < new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 15, 56, 0))
            );
        }

        public static DateTime NewYearsDay(int year)
        {
            return new DateTime(year, 1, 1);
        }

        public static DateTime MartinLutherKingJrDay(int year)
        {
            var startDate = new DateTime(year, 1, 1);

            int multiplier = startDate.DayOfWeek != DayOfWeek.Monday ? 3 : 2;

            return startDate.AddDays(multiplier * 7);
        }


    }
}
