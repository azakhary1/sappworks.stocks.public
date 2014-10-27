
namespace Sappworks.Stocks
{
    using System;
    using System.Linq;

    public static class DateTimeExtensions
    {
        public static bool IsTradingHour(this DateTime value)
        { 
            var currentTime = DateTime.Now;

            if (currentTime.Month == 1 && currentTime.Date == NewYearsDay(currentTime.Year)) return false;

            if (currentTime.Month == 1 && currentTime.Date == MartinLutherKingJrDay(currentTime.Year)) return false;

            if (currentTime.Month == 2 && currentTime.Date == PresidentsDay(currentTime.Year)) return false;

            if (currentTime.Between(new DateTime(currentTime.Year, 3, 22), new DateTime(currentTime.Year, 4, 25)) && currentTime.Date == GoodFriday(currentTime.Year)) return false;

            if (currentTime.Month == 5 && currentTime.Date == MemorialDay(currentTime.Year)) return false;

            if (
                currentTime.Month == 7
                && (
                    currentTime.Date == JulyThird(currentTime.Year)
                    || currentTime.Date == IndependenceDay(currentTime.Year)
                )
            ) return false;

            if (currentTime.Month == 9 && currentTime.Date == LaborDay(currentTime.Year)) return false;

            if (
                currentTime.Month == 11
                && (
                    currentTime.Date == ThanksgivingDay(currentTime.Year)
                    || currentTime.Date == DayAfterThanksgivingDay(currentTime.Year)
                )
            ) return false;

            if (
                currentTime.Month == 12
                && (
                    currentTime.Date == ChristmasEve(currentTime.Year)
                    || currentTime.Date == ChristmasDay(currentTime.Year)
                )
            ) return false; 

            return (
                (
                    currentTime.DayOfWeek != DayOfWeek.Sunday 
                    && currentTime.DayOfWeek != DayOfWeek.Saturday
                )
                && (currentTime > new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 9, 32, 0)) 
                && (currentTime < new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 15, 58, 0))
            );
        }

        public static DateTime NewYearsDay(int year)
        {
            return new DateTime(year, 1, 1);
        }

        /// <summary>
        /// Third monday in January
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime MartinLutherKingJrDay(int year)
        {
            return NthWeekdayOfMonth(year, 1, 3, DayOfWeek.Monday);
        }

        /// <summary>
        /// Third monday in February
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime PresidentsDay(int year)
        {
            return NthWeekdayOfMonth(year, 2, 3, DayOfWeek.Monday);
        }

        /// <summary>
        /// Last monday in may
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime MemorialDay(int year)
        {
            var startDate = new DateTime(year, 5, 31);

            if (startDate.DayOfWeek == DayOfWeek.Monday)
            {
                return startDate;            
            }

            while (startDate.DayOfWeek != DayOfWeek.Monday)
            {
                startDate.AddDays(-1);
            }

            return startDate;
        }

        public static DateTime EasterSunday(int year)
        {
            // WTF PEOPLE !!!!
            // http://aa.usno.navy.mil/faq/docs/easter.php

            int day = 0;
            int month = 0;

            int g = year % 19;
            int c = year / 100;
            int h = (c - (int)(c / 4) - (int)((8 * c + 13) / 25) + 19 * g + 15) % 30;
            int i = h - (int)(h / 28) * (1 - (int)(h / 28) * (int)(29 / (h + 1)) * (int)((21 - g) / 11));

            day = i - ((year + (int)(year / 4) + i + 2 - c + (int)(c / 4)) % 7) + 28;
            month = 3;

            if (day > 31)
            {
                month++;
                day -= 31;
            }

            return new DateTime(year, month, day);
        }

        public static DateTime GoodFriday(int year)
        {
            return EasterSunday(year).AddDays(-2);
        }

        public static DateTime JulyThird(int year)
        {
            return new DateTime(year, 7, 3);
        }

        public static DateTime IndependenceDay(int year)
        {
            return new DateTime(year, 7, 4);
        }

        public static DateTime LaborDay(int year)
        {
            return NthWeekdayOfMonth(year, 9, 1, DayOfWeek.Monday);
        }

        public static DateTime ThanksgivingDay(int year)
        {
            return NthWeekdayOfMonth(year, 11, 4, DayOfWeek.Thursday);
        }

        public static DateTime DayAfterThanksgivingDay(int year)
        {
            return ThanksgivingDay(year).AddDays(1);
        }

        public static DateTime ChristmasEve(int year)
        {
            return new DateTime(year, 12, 24);
        }

        public static DateTime ChristmasDay(int year)
        {
            return new DateTime(year, 12, 25);
        }

        public static bool Between(this DateTime value, DateTime a, DateTime b)
        {
            var array = new[] { a, b };

            var lower = array.Min();
            
            var upper = array.Max();

            return ((lower < value) && (value < upper));
        }

        public static DateTime Next(this DateTime date, DayOfWeek dayOfWeek) 
        { 
            return date.AddDays((dayOfWeek < date.DayOfWeek ? 7 : 0) + dayOfWeek - date.DayOfWeek); 
        }

        public static DateTime NthWeekdayOfMonth(int year, int month, int nthWeek, DayOfWeek dayOfWeek)
        {
            return new DateTime(year, month, 1).Next(dayOfWeek).AddDays((nthWeek - 1) * 7);
        }
    }
}
