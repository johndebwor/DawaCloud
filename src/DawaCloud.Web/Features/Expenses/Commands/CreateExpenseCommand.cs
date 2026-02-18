using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Expenses.Commands;

public record CreateExpenseCommand(
    DateTime Date,
    int CategoryId,
    string Description,
    decimal Amount,
    int? CurrencyId,
    PaymentMethod PaymentMethod,
    string? VendorName,
    string? Notes,
    bool IsRecurring,
    RecurrenceFrequency RecurrenceFrequency,
    int RecurrenceInterval,
    int? WeeklyDays,
    MonthlyRecurrenceType? MonthlyRecurrenceType,
    int? MonthlyDayOfMonth,
    DayOfWeek? MonthlyDayOfWeek,
    WeekPosition? MonthlyWeekPosition,
    RecurrenceEndType RecurrenceEndType,
    DateTime? RecurrenceEndDate,
    int? RecurrenceEndAfterCount
) : IRequest<Result<int>>;

public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, Result<int>>
{
    private readonly AppDbContext _context;

    public CreateExpenseCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateExpenseCommand request, CancellationToken ct)
    {
        var maxId = await _context.Expenses
            .IgnoreQueryFilters()
            .OrderByDescending(e => e.Id)
            .Select(e => e.Id)
            .FirstOrDefaultAsync(ct);

        var nextNumber = maxId + 1;
        var referenceNumber = $"EXP-{DateTime.UtcNow.Year}-{nextNumber:D5}";

        var amountBase = request.Amount;
        decimal? exchangeRate = null;

        if (request.CurrencyId.HasValue)
        {
            var rate = await _context.ExchangeRates
                .Where(er => er.FromCurrencyId == request.CurrencyId.Value && er.IsActive)
                .OrderByDescending(er => er.EffectiveDate)
                .FirstOrDefaultAsync(ct);

            if (rate != null)
            {
                exchangeRate = rate.Rate;
                amountBase = request.Amount * rate.Rate;
            }
        }

        var expense = new Expense
        {
            ReferenceNumber = referenceNumber,
            Date = request.Date,
            CategoryId = request.CategoryId,
            Description = request.Description,
            Amount = request.Amount,
            CurrencyId = request.CurrencyId,
            ExchangeRateUsed = exchangeRate,
            AmountBase = amountBase,
            PaymentMethod = request.PaymentMethod,
            VendorName = request.VendorName,
            Notes = request.Notes,
            IsRecurring = request.IsRecurring,

            // Recurrence configuration
            RecurrenceFrequency = request.RecurrenceFrequency,
            RecurrenceInterval = request.RecurrenceInterval,
            WeeklyDays = request.WeeklyDays,
            MonthlyRecurrenceType = request.MonthlyRecurrenceType,
            MonthlyDayOfMonth = request.MonthlyDayOfMonth,
            MonthlyDayOfWeek = request.MonthlyDayOfWeek,
            MonthlyWeekPosition = request.MonthlyWeekPosition,
            RecurrenceEndType = request.RecurrenceEndType,
            RecurrenceEndDate = request.RecurrenceEndDate,
            RecurrenceEndAfterCount = request.RecurrenceEndAfterCount,

            // Template tracking (this is the template if recurring)
            RecurrenceTemplateId = null,
            OccurrenceNumber = request.IsRecurring ? 0 : 0,

            Status = ExpenseStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync(ct);

        return Result<int>.Ok(expense.Id);
    }
}
