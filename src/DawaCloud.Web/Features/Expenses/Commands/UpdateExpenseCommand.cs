using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Expenses.Commands;

public record UpdateExpenseCommand(
    int Id,
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
) : IRequest<Result<bool>>;

public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, Result<bool>>
{
    private readonly AppDbContext _context;

    public UpdateExpenseCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateExpenseCommand request, CancellationToken ct)
    {
        var expense = await _context.Expenses.FindAsync([request.Id], ct);
        if (expense == null)
            return Result<bool>.Fail("Expense not found");

        if (expense.Status != ExpenseStatus.Pending)
            return Result<bool>.Fail("Only pending expenses can be edited");

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

        expense.Date = request.Date;
        expense.CategoryId = request.CategoryId;
        expense.Description = request.Description;
        expense.Amount = request.Amount;
        expense.CurrencyId = request.CurrencyId;
        expense.ExchangeRateUsed = exchangeRate;
        expense.AmountBase = amountBase;
        expense.PaymentMethod = request.PaymentMethod;
        expense.VendorName = request.VendorName;
        expense.Notes = request.Notes;
        expense.IsRecurring = request.IsRecurring;

        // Update recurrence configuration
        expense.RecurrenceFrequency = request.RecurrenceFrequency;
        expense.RecurrenceInterval = request.RecurrenceInterval;
        expense.WeeklyDays = request.WeeklyDays;
        expense.MonthlyRecurrenceType = request.MonthlyRecurrenceType;
        expense.MonthlyDayOfMonth = request.MonthlyDayOfMonth;
        expense.MonthlyDayOfWeek = request.MonthlyDayOfWeek;
        expense.MonthlyWeekPosition = request.MonthlyWeekPosition;
        expense.RecurrenceEndType = request.RecurrenceEndType;
        expense.RecurrenceEndDate = request.RecurrenceEndDate;
        expense.RecurrenceEndAfterCount = request.RecurrenceEndAfterCount;

        expense.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}
