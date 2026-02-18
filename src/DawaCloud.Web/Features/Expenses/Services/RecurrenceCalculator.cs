using DawaCloud.Web.Data.Entities;

namespace DawaCloud.Web.Features.Expenses.Services;

public class RecurrenceCalculator
{
    /// <summary>
    /// Gets the next N occurrences based on recurrence configuration
    /// </summary>
    public List<DateTime> GetNextOccurrences(
        DateTime startDate,
        RecurrenceConfiguration config,
        int count,
        int startFromOccurrence = 1)
    {
        var dates = new List<DateTime>();
        var currentDate = startDate;
        int occurrenceNum = 0;

        while (dates.Count < count)
        {
            currentDate = GetNextOccurrence(currentDate, config, occurrenceNum == 0);
            occurrenceNum++;

            if (occurrenceNum < startFromOccurrence)
                continue;

            // Check end conditions
            if (config.EndType == RecurrenceEndType.OnDate &&
                config.EndDate.HasValue &&
                currentDate > config.EndDate.Value)
                break;

            if (config.EndType == RecurrenceEndType.AfterCount &&
                config.EndAfterCount.HasValue &&
                occurrenceNum >= config.EndAfterCount.Value)
                break;

            dates.Add(currentDate);
        }

        return dates;
    }

    /// <summary>
    /// Gets the human-readable summary of the recurrence pattern
    /// </summary>
    public string GetRecurrenceSummary(RecurrenceConfiguration config)
    {
        if (config.Frequency == RecurrenceFrequency.None)
            return "Does not repeat";

        var parts = new List<string>();

        // Frequency and interval
        string frequencyText = config.Frequency switch
        {
            RecurrenceFrequency.Daily => config.Interval == 1 ? "daily" : $"every {config.Interval} days",
            RecurrenceFrequency.Weekly => config.Interval == 1 ? "weekly" : $"every {config.Interval} weeks",
            RecurrenceFrequency.Monthly => config.Interval == 1 ? "monthly" : $"every {config.Interval} months",
            RecurrenceFrequency.Yearly => config.Interval == 1 ? "yearly" : $"every {config.Interval} years",
            _ => ""
        };
        parts.Add("Repeats " + frequencyText);

        // Weekly days
        if (config.Frequency == RecurrenceFrequency.Weekly && config.WeeklyDays.HasValue)
        {
            var days = GetSelectedDayNames(config.WeeklyDays.Value);
            if (days.Any())
                parts.Add("on " + string.Join(", ", days));
        }

        // Monthly pattern
        if (config.Frequency == RecurrenceFrequency.Monthly)
        {
            if (config.MonthlyType == MonthlyRecurrenceType.DayOfMonth && config.MonthlyDayOfMonth.HasValue)
            {
                parts.Add($"on day {config.MonthlyDayOfMonth}");
            }
            else if (config.MonthlyType == MonthlyRecurrenceType.DayOfWeek &&
                     config.MonthlyWeekPosition.HasValue &&
                     config.MonthlyDayOfWeek.HasValue)
            {
                var position = config.MonthlyWeekPosition.Value.ToString().ToLower();
                var day = config.MonthlyDayOfWeek.Value.ToString();
                parts.Add($"on the {position} {day}");
            }
        }

        // End condition
        if (config.EndType == RecurrenceEndType.OnDate && config.EndDate.HasValue)
        {
            parts.Add($"until {config.EndDate.Value:MMM d, yyyy}");
        }
        else if (config.EndType == RecurrenceEndType.AfterCount && config.EndAfterCount.HasValue)
        {
            parts.Add($"for {config.EndAfterCount} occurrence{(config.EndAfterCount > 1 ? "s" : "")}");
        }

        return string.Join(", ", parts);
    }

    private DateTime GetNextOccurrence(DateTime currentDate, RecurrenceConfiguration config, bool isFirst)
    {
        if (isFirst)
            return currentDate;

        return config.Frequency switch
        {
            RecurrenceFrequency.Daily => currentDate.AddDays(config.Interval),
            RecurrenceFrequency.Weekly => GetNextWeeklyOccurrence(currentDate, config),
            RecurrenceFrequency.Monthly => GetNextMonthlyOccurrence(currentDate, config),
            RecurrenceFrequency.Yearly => currentDate.AddYears(config.Interval),
            _ => currentDate
        };
    }

    private DateTime GetNextWeeklyOccurrence(DateTime currentDate, RecurrenceConfiguration config)
    {
        if (!config.WeeklyDays.HasValue || config.WeeklyDays.Value == 0)
            return currentDate.AddDays(7 * config.Interval);

        // Find next selected day
        for (int i = 1; i <= 7 * config.Interval; i++)
        {
            var testDate = currentDate.AddDays(i);
            int dayBit = 1 << (int)testDate.DayOfWeek;

            if ((config.WeeklyDays.Value & dayBit) != 0)
                return testDate;
        }

        return currentDate.AddDays(7 * config.Interval);
    }

    private DateTime GetNextMonthlyOccurrence(DateTime currentDate, RecurrenceConfiguration config)
    {
        if (config.MonthlyType == MonthlyRecurrenceType.DayOfMonth)
        {
            var nextMonth = currentDate.AddMonths(config.Interval);
            int targetDay = Math.Min(
                config.MonthlyDayOfMonth ?? currentDate.Day,
                DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
            return new DateTime(nextMonth.Year, nextMonth.Month, targetDay);
        }
        else // DayOfWeek
        {
            var nextMonth = currentDate.AddMonths(config.Interval);
            return GetNthDayOfMonth(
                nextMonth.Year,
                nextMonth.Month,
                config.MonthlyDayOfWeek ?? DayOfWeek.Monday,
                config.MonthlyWeekPosition ?? WeekPosition.First);
        }
    }

    private DateTime GetNthDayOfMonth(int year, int month, DayOfWeek dayOfWeek, WeekPosition position)
    {
        var firstDay = new DateTime(year, month, 1);
        var lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));

        if (position == WeekPosition.Last)
        {
            // Start from end of month
            var date = lastDay;
            while (date.DayOfWeek != dayOfWeek)
                date = date.AddDays(-1);
            return date;
        }
        else
        {
            // Start from beginning
            var date = firstDay;
            while (date.DayOfWeek != dayOfWeek)
                date = date.AddDays(1);

            // Add weeks for 2nd, 3rd, 4th
            int weeksToAdd = position switch
            {
                WeekPosition.Second => 1,
                WeekPosition.Third => 2,
                WeekPosition.Fourth => 3,
                _ => 0
            };

            return date.AddDays(7 * weeksToAdd);
        }
    }

    private List<string> GetSelectedDayNames(int weeklyDays)
    {
        var days = new List<string>();
        var dayNames = new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

        for (int i = 0; i < 7; i++)
        {
            if ((weeklyDays & (1 << i)) != 0)
                days.Add(dayNames[i]);
        }

        return days;
    }
}

/// <summary>
/// Configuration for recurrence calculation
/// </summary>
public class RecurrenceConfiguration
{
    public RecurrenceFrequency Frequency { get; set; }
    public int Interval { get; set; } = 1;
    public int? WeeklyDays { get; set; }
    public MonthlyRecurrenceType? MonthlyType { get; set; }
    public int? MonthlyDayOfMonth { get; set; }
    public DayOfWeek? MonthlyDayOfWeek { get; set; }
    public WeekPosition? MonthlyWeekPosition { get; set; }
    public RecurrenceEndType EndType { get; set; }
    public DateTime? EndDate { get; set; }
    public int? EndAfterCount { get; set; }

    public static RecurrenceConfiguration FromExpense(Expense expense)
    {
        return new RecurrenceConfiguration
        {
            Frequency = expense.RecurrenceFrequency,
            Interval = expense.RecurrenceInterval,
            WeeklyDays = expense.WeeklyDays,
            MonthlyType = expense.MonthlyRecurrenceType,
            MonthlyDayOfMonth = expense.MonthlyDayOfMonth,
            MonthlyDayOfWeek = expense.MonthlyDayOfWeek,
            MonthlyWeekPosition = expense.MonthlyWeekPosition,
            EndType = expense.RecurrenceEndType,
            EndDate = expense.RecurrenceEndDate,
            EndAfterCount = expense.RecurrenceEndAfterCount
        };
    }
}
