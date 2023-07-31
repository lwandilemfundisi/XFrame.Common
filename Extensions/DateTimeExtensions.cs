using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace XFrame.Common.Extensions
{
    public static class DateTimeExtensions
    {
        private enum Month
        {
            None = 0,
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12
        }

        private enum Quarter
        {
            None = 0,
            First = 1,
            Second = 2,
            Third = 3,
            Fourth = 4
        }

        public static long ToUnixTime(this DateTimeOffset dateTimeOffset)
        {
            return Convert.ToInt64((dateTimeOffset.UtcDateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
        }

        public static string AsCalculatorShortDate(this DateTime? value)
        {
            if (value.IsNull())
            {
                return "&nbsp;";
            }

            return value.Value.AsCalculatorShortDate();
        }

        public static string AsCalculatorShortDate(this DateTime value)
        {
            return value.AsString("{0:dd/MM/yyyy}");
        }

        public static string DurationTime(this DateTime value, DateTime endDate)
        {
            var duration = endDate.Subtract(value);

            var days = (duration.Days > 0) ? "{0} Days".FormatInvariantCulture(duration.Days) : string.Empty;
            var hours = (duration.Hours > 0) ? "{0} Hours".FormatInvariantCulture(duration.Hours) : string.Empty;
            var minutes = (duration.Minutes > 0) ? "{0} Min".FormatInvariantCulture(duration.Minutes) : string.Empty;
            var seconds = (duration.Seconds > 0) ? "{0}s".FormatInvariantCulture(duration.Seconds) : string.Empty;
            var milliseconds = (duration.Milliseconds > 0) ? "{0}ms".FormatInvariantCulture(duration.Milliseconds) : string.Empty;

            var result = "{0} {1} {2} {3} {4}".FormatInvariantCulture(days, hours, minutes, seconds, milliseconds);

            return result.Replace(" ", string.Empty).IsNotNullOrEmpty() ? result : null;
        }

        public static DateTime PreviousWorkingDay(this DateTime value, int daysInPast, List<DateTime> publicHolidays, bool inclusive = false)
        {
            Invariant.IsTrue(daysInPast > 0, () => "daysInPast cannot be 0");

            var totalWorkingDays = 0;
            var workingDay = value;

            if (inclusive)
            {
                workingDay = value.Date.AddDays(1);
            }

            while (totalWorkingDays < daysInPast)
            {
                workingDay = workingDay.AddDays(-1);

                if (workingDay.IsWorkingDay() && !publicHolidays.Contains(c => c.Date == workingDay))
                {
                    totalWorkingDays++;
                }
            }

            return workingDay;
        }

        public static DateTime NextWorkingDay(this DateTime value, int daysInFuture, List<DateTime> publicHolidays)
        {
            Invariant.IsTrue(daysInFuture > 0, () => "daysInFuture cannot be 0");

            var totalWorkingDays = 0;
            var workingDay = value;

            while (totalWorkingDays < daysInFuture)
            {
                if (workingDay.IsWorkingDay() && !publicHolidays.Contains(c => c.Date == workingDay))
                {
                    totalWorkingDays++;
                }

                if (totalWorkingDays != daysInFuture)
                {
                    workingDay = workingDay.AddDays(1);
                }
            }

            return workingDay;
        }

        public static int TotalWorkingDays(this DateTime value, double addedDays)
        {
            var currentDay = 1;
            var totalWorkingDays = 0;

            while (currentDay <= addedDays)
            {
                if (value.AddDays(currentDay).IsWorkingDay())
                {
                    totalWorkingDays++;
                }

                currentDay++;
            }

            return totalWorkingDays;
        }

        public static DateTime AddWorkingDays(this DateTime value, double days)
        {
            Invariant.IsFalse(days < 1, () => "days must be greater or equal to 1");

            var totalDays = 0;
            var totalWorkingDays = 0;

            while (totalWorkingDays < days + 1)
            {
                if (value.AddDays(totalDays).IsWorkingDay())
                {
                    totalWorkingDays++;
                }

                totalDays++;
            }

            return value.AddDays(totalDays - 1);
        }

        public static int? SafeCalculateAgeNextBirthday(this DateTime? value)
        {
            if (value.HasValue)
            {
                return value.Value.CalculateAgeNextBirthday();
            }

            return null;
        }

        public static int CalculateAgeNextBirthday(this DateTime value)
        {
            if (value > DateTime.Now)
            {
                return 0;
            }

            var age = DateTime.Now.Year - value.Year;
            string nextBirthdayString;

            if (value.Month == 2 && value.Day == 29)
            {
                nextBirthdayString = "1/3/" + DateTime.Now.Year;
            }
            else
            {
                nextBirthdayString = value.Day + "/" + value.Month + "/" + DateTime.Now.Year;
            }

            var nextBirthday = DateTime.ParseExact(nextBirthdayString, "d/M/yyyy", CultureInfo.InvariantCulture);
            var ageNextBirthday = DateTime.Now < nextBirthday ? age : age + 1;
            return ageNextBirthday;
        }

        public static int CalculateAgeAtDate(this DateTime dateOfBirth, DateTime targetDate)
        {
            if (dateOfBirth > targetDate)
            {
                return 0;
            }

            var newTargetDate = new DateTime(2000, targetDate.Month, targetDate.Day);
            var newDateOfBirth = new DateTime(2000, dateOfBirth.Month, dateOfBirth.Day);

            if (newTargetDate >= newDateOfBirth)
            {
                return (targetDate.Year - dateOfBirth.Year);
            }

            return (targetDate.Year - dateOfBirth.Year - 1);
        }

        public static int? SafeCalculateCurrentAge(this DateTime? value)
        {
            if (value.HasValue)
            {
                var currentDate = DateTime.Now;
                var dob = value.GetValueOrDefault();
                int age = currentDate.Year - dob.Year;
                if (currentDate.Month > dob.Month)
                {
                    return age;
                }
                if (currentDate.Month < dob.Month)
                {
                    return age - 1;
                }
                if (currentDate.Month == dob.Month && currentDate.Day < dob.Day)
                {
                    return age - 1;
                };

                return age;
            }

            return null;
        }

        public static int MonthDifference(this DateTime value, DateTime subtraction)
        {
            return Math.Abs((value.Month - subtraction.Month) + 12 * (value.Year - subtraction.Year));
        }

        public static bool IsWeekend(this DateTime value)
        {
            switch (value.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    {
                        return true;
                    }
            }

            return false;
        }

        public static bool IsWorkingDay(this DateTime value)
        {
            return !value.IsWeekend();
        }

        public static DateTime NextDay(this DateTime value, DayOfWeek targetDay)
        {
            var targetDate = value;

            while (targetDate.DayOfWeek != targetDay)
            {
                targetDate = targetDate.AddDays(1);
            }

            return targetDate;
        }

        public static DateTime PreviousDay(this DateTime value, DayOfWeek targetDay)
        {
            var targetDate = value;

            while (targetDate.DayOfWeek != targetDay)
            {
                targetDate = targetDate.AddDays(-1);
            }

            return targetDate;
        }

        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        public static DateTime LastDayOfMonth(this DateTime value)
        {
            return value.AddMonths(1).FirstDayOfMonth().AddDays(-1);
        }

        public static DateTime NextMonth(this DateTime value)
        {
            return value.LastDayOfMonth().AddDays(1);
        }

        public static DateTime? LastDayOfMonth(this DateTime? value)
        {
            if (value.IsNotNull())
            {
                return value.Value.LastDayOfMonth();
            }

            return null;
        }

        public static DateTime LastDayOfYear(this DateTime value)
        {
            return new DateTime(value.Year, 12, 31);
        }

        public static bool Between(this DateTime value, DateTime? startDate, DateTime? endDate)
        {
            return value >= startDate.GetValueOrDefault(DateTime.MinValue) && value <= endDate.GetValueOrDefault(DateTime.MaxValue);
        }

        public static bool Between(this DateTime value, DateTime startDate, DateTime endDate)
        {
            return value >= startDate && value <= endDate;
        }

        public static List<T> ItemsInRange<T>(this IEnumerable<T> source, Func<T, DateTime?> startDate, Func<T, DateTime?> endDate, DateTime? rangeStartDate, DateTime? rangeEndDate)
        {
            var result = new List<T>();

            if (source.HasItems())
            {
                foreach (var sourceItem in source)
                {
                    var sourceStartDate = startDate.Invoke(sourceItem);
                    var sourceEndDate = endDate.Invoke(sourceItem);

                    if ((sourceStartDate >= rangeStartDate && sourceStartDate <= rangeEndDate)
                        && (sourceEndDate <= rangeEndDate && sourceEndDate >= rangeStartDate))
                    {
                        result.Add(sourceItem);
                    }
                }
            }

            return result;
        }

        public static bool DatesOverlap<T>(this IEnumerable<T> result, Func<T, DateTime?> startDate, Func<T, DateTime?> endDate)
        {
            var datesOverlap = false;

            foreach (var period in result)
            {
                foreach (var comparisonDate in result)
                {
                    if (!ReferenceEquals(period, comparisonDate))
                    {
                        if (TimePeriodOverlap(period, comparisonDate, startDate, endDate))
                        {
                            datesOverlap = true;
                            break;
                        }
                    }
                }

                if (!datesOverlap)
                {
                    break;
                }
            }

            return datesOverlap;
        }

        public static DateTime StartOfDay(this DateTime value)
        {
            return value.Date;
        }

        public static DateTime EndOfDay(this DateTime value)
        {
            return value.Date.AddDays(1).AddTicks(-1);
        }

        public static DateTime TaxYearStart(this DateTime value, int taxYearStartMonth)
        {
            var initialTaxStartDate = new DateTime(value.Year, taxYearStartMonth, 1);

            if (initialTaxStartDate <= value)
            {
                return initialTaxStartDate;
            }

            return initialTaxStartDate.AddYears(-1);
        }

        public static DateTime TaxYearEnd(this DateTime value, int taxYearStartMonth)
        {
            return value.TaxYearStart(taxYearStartMonth).AddYears(1).AddDays(-1);
        }

        public static string AsShortDate(this DateTime value)
        {
            return value.AsString("{0:dd/MM/yyyy}");
        }

        public static string AsDashShortDate(this DateTime value)
        {
            return value.AsString("{0:dd-MM-yyyy}");
        }

        public static string AsShortDate(this DateTime? value)
        {
            return value.AsShortDate("&nbsp;");
        }

        public static string AsShortDate(this DateTime? value, string defaultValue)
        {
            if (value.IsNull())
            {
                return defaultValue;
            }

            return value.Value.AsShortDate();
        }

        public static string AsShortDateTime(this DateTime? value)
        {
            if (value.IsNull())
            {
                return "&nbsp;";
            }

            return value.Value.AsString("{0:dd/MM/yyyy HH:mm}");
        }

        public static string AsLongDate(this DateTime value)
        {
            return value.AsLongDate(' ');
        }

        public static string AsLongDate(this DateTime value, char seperator)
        {
            return value.AsString("{0:dd" + seperator + "MMMM" + seperator + "yyyy}");
        }

        public static string AsLongDate(this DateTime? value)
        {
            if (value.IsNull())
            {
                return "&nbsp";
            }
            return value.Value.AsLongDate(' ');
        }

        public static string AsLongDate(this DateTime? value, char seperator)
        {
            if (value.IsNull())
            {
                return "&nbsp";
            }
            return value.Value.AsLongDate(seperator);
        }

        public static string AsAbbreviatedDate(this DateTime value, char separator)
        {
            return value.AsString("{0:dd" + separator + "MMM" + separator + "yyyy}");
        }

        public static string AsAbbreviatedDate(this DateTime? value, char separator, string defaultValue)
        {
            if (value.IsNull())
            {
                return defaultValue;
            }
            return value.GetValueOrDefault().AsAbbreviatedDate(separator);
        }

        public static string AsAbbreviatedDateTime(this DateTime value, char separator)
        {
            return value.AsString("{0:dd" + separator + "MMM" + separator + "yyyy HH:mm}");
        }

        public static DateTime EndOfPreviousMonth(this DateTime value)
        {
            return value.AddDays(value.Day * -1).Date;
        }

        public static DateTime EndOfQuarter(this DateTime value)
        {
            return GetEndOfQuarter(value.Year, GetQuarter((Month)value.Month)).Date;
        }

        public static DateTime EndOfPreviousQuarter(this DateTime value)
        {
            if ((Month)value.Month <= Month.March)
            {
                return GetEndOfQuarter(value.Year - 1, Quarter.Fourth);
            }
            else
            {
                return GetEndOfQuarter(value.Year, GetQuarter((Month)value.Month) - 1);
            }
        }

        public static bool IsFirstHalfOfMonth(DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.Day < 15;
            }

            return DateTime.Today.Day < 15;
        }

        public static bool IsSecondHalfOfMonth(DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.Day >= 15;
            }

            return DateTime.Today.Day >= 15;
        }


        #region Private Methods

        private static bool TimePeriodOverlap<T>(T dateOne, T dateTwo, Func<T, DateTime?> startDate, Func<T, DateTime?> endDate)
        {
            var dateOneTaxStartDate = startDate.Invoke(dateOne);
            var dateOneTaxEndDate = endDate.Invoke(dateOne);

            var dateTwoTaxStartDate = startDate.Invoke(dateTwo);
            var dateTwoTaxEndDate = endDate.Invoke(dateTwo);

            return (dateTwoTaxStartDate >= dateOneTaxStartDate && dateTwoTaxStartDate < dateOneTaxEndDate)
                || (dateTwoTaxEndDate <= dateOneTaxEndDate && dateTwoTaxEndDate > dateOneTaxStartDate)
                || (dateTwoTaxStartDate <= dateOneTaxStartDate && dateTwoTaxEndDate >= dateOneTaxEndDate);
        }



        private static Quarter GetQuarter(Month month)
        {
            if (month <= Month.March)
            {
                return Quarter.First;
            }
            else if ((month >= Month.April) && (month <= Month.June))
            {
                return Quarter.Second;
            }
            else if ((month >= Month.July) && (month <= Month.September))
            {

                return Quarter.Third;
            }
            else
            {
                return Quarter.Fourth;
            }
        }

        private static DateTime GetEndOfQuarter(int Year, Quarter quarter)
        {
            if (quarter == Quarter.First)
            {
                return new DateTime(Year, 3, DateTime.DaysInMonth(Year, 3)).Date;
            }
            else if (quarter == Quarter.Second)
            {
                return new DateTime(Year, 6, DateTime.DaysInMonth(Year, 6)).Date;
            }
            else if (quarter == Quarter.Third)
            {
                return new DateTime(Year, 9, DateTime.DaysInMonth(Year, 9)).Date;
            }
            else
            {
                return new DateTime(Year, 12, DateTime.DaysInMonth(Year, 12)).Date;
            }
        }


        #endregion

    }
}
