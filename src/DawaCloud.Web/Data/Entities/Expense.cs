namespace DawaCloud.Web.Data.Entities;

// Recurrence Enums
public enum RecurrenceFrequency
{
    None,       // Does not repeat
    Daily,      // Every day
    Weekly,     // Every week
    Monthly,    // Every month
    Yearly      // Every year
}

public enum RecurrenceEndType
{
    Never,          // No end date
    OnDate,         // Ends on specific date
    AfterCount      // Ends after X occurrences
}

public enum MonthlyRecurrenceType
{
    DayOfMonth,     // e.g., "15th of every month"
    DayOfWeek       // e.g., "Last Friday of every month"
}

public enum WeekPosition
{
    First,
    Second,
    Third,
    Fourth,
    Last
}

public class ExpenseCategory : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? BudgetLimit { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}

public class Expense : BaseAuditableEntity
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int CategoryId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int? CurrencyId { get; set; }
    public decimal? ExchangeRateUsed { get; set; }
    public decimal AmountBase { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? ReceiptUrl { get; set; }
    public string? VendorName { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public ExpenseStatus Status { get; set; } = ExpenseStatus.Pending;

    // Basic recurrence
    public bool IsRecurring { get; set; }

    // Recurrence configuration
    public RecurrenceFrequency RecurrenceFrequency { get; set; } = RecurrenceFrequency.None;
    public int RecurrenceInterval { get; set; } = 1;  // Every X days/weeks/months/years

    // Weekly recurrence: bit flags for days (0=Sunday, 1=Monday, ..., 6=Saturday)
    public int? WeeklyDays { get; set; }  // Bitmask: 0b0111110 = Mon-Fri

    // Monthly recurrence
    public MonthlyRecurrenceType? MonthlyRecurrenceType { get; set; }
    public int? MonthlyDayOfMonth { get; set; }  // 1-31
    public DayOfWeek? MonthlyDayOfWeek { get; set; }
    public WeekPosition? MonthlyWeekPosition { get; set; }  // First, Second, Last, etc.

    // End conditions
    public RecurrenceEndType RecurrenceEndType { get; set; } = RecurrenceEndType.Never;
    public DateTime? RecurrenceEndDate { get; set; }
    public int? RecurrenceEndAfterCount { get; set; }

    // Template tracking (for generated instances)
    public int? RecurrenceTemplateId { get; set; }  // If this is generated from template
    public int OccurrenceNumber { get; set; }       // Which occurrence in series (0 = template)

    // Deprecated (kept for backward compatibility)
    [Obsolete("Use RecurrenceFrequency and related fields instead")]
    public string? RecurrencePattern { get; set; }

    public string? Notes { get; set; }

    // Navigation
    public ExpenseCategory Category { get; set; } = null!;
    public Currency? Currency { get; set; }
    public Expense? RecurrenceTemplate { get; set; }  // Parent template
    public ICollection<Expense> GeneratedInstances { get; set; } = new List<Expense>();
}

public enum ExpenseStatus
{
    Pending,
    Approved,
    Rejected,
    Voided
}
