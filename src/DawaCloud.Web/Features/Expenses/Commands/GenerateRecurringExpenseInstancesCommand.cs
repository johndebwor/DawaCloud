using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Features.Expenses.Services;
using DawaCloud.Web.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Expenses.Commands;

/// <summary>
/// Generates future expense instances from a recurring expense template
/// </summary>
public record GenerateRecurringExpenseInstancesCommand(
    int TemplateExpenseId,
    int? MaxInstancesToGenerate = 12  // Generate up to 12 months ahead by default
) : IRequest<Result<int>>;

public class GenerateRecurringExpenseInstancesCommandHandler
    : IRequestHandler<GenerateRecurringExpenseInstancesCommand, Result<int>>
{
    private readonly AppDbContext _context;
    private readonly RecurrenceCalculator _calculator;

    public GenerateRecurringExpenseInstancesCommandHandler(AppDbContext context)
    {
        _context = context;
        _calculator = new RecurrenceCalculator();
    }

    public async Task<Result<int>> Handle(
        GenerateRecurringExpenseInstancesCommand request,
        CancellationToken ct)
    {
        // Load the template expense with generated instances
        var template = await _context.Expenses
            .Include(e => e.GeneratedInstances)
            .FirstOrDefaultAsync(e => e.Id == request.TemplateExpenseId, ct);

        if (template == null)
            return Result<int>.Fail("Template expense not found");

        if (!template.IsRecurring || template.RecurrenceFrequency == RecurrenceFrequency.None)
            return Result<int>.Fail("Expense is not a recurring template");

        // Build recurrence configuration
        var config = RecurrenceConfiguration.FromExpense(template);

        // Get last generated occurrence number
        int lastOccurrence = template.GeneratedInstances.Any()
            ? template.GeneratedInstances.Max(i => i.OccurrenceNumber)
            : 0;

        // Calculate future dates
        var futureDates = _calculator.GetNextOccurrences(
            template.Date,
            config,
            request.MaxInstancesToGenerate ?? 12,
            startFromOccurrence: lastOccurrence + 1
        );

        int generatedCount = 0;

        foreach (var (date, occurrenceNum) in futureDates.Select((d, i) => (d, lastOccurrence + i + 1)))
        {
            // Don't create if already exists
            if (template.GeneratedInstances.Any(i => i.OccurrenceNumber == occurrenceNum))
                continue;

            // Don't generate expenses in the past
            if (date < DateTime.Today)
                continue;

            var instance = new Expense
            {
                ReferenceNumber = $"EXP-{date.Year}-{Guid.NewGuid().ToString()[..5].ToUpper()}",
                Date = date,
                CategoryId = template.CategoryId,
                Description = $"{template.Description} (Occurrence {occurrenceNum})",
                Amount = template.Amount,
                CurrencyId = template.CurrencyId,
                ExchangeRateUsed = template.ExchangeRateUsed,
                AmountBase = template.AmountBase,
                PaymentMethod = template.PaymentMethod,
                VendorName = template.VendorName,
                Notes = template.Notes,
                IsRecurring = false,  // Instances are not templates
                RecurrenceFrequency = RecurrenceFrequency.None,
                RecurrenceTemplateId = template.Id,
                OccurrenceNumber = occurrenceNum,
                Status = ExpenseStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Expenses.Add(instance);
            generatedCount++;
        }

        if (generatedCount > 0)
        {
            await _context.SaveChangesAsync(ct);
        }

        return Result<int>.Ok(generatedCount);
    }
}
