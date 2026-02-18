using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Expenses.Queries;

public record GetExpensesQuery(
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    int? CategoryId = null,
    ExpenseStatus? Status = null,
    string? SearchTerm = null
) : IRequest<ExpenseListResult>;

public record ExpenseListResult(
    List<ExpenseDto> Items,
    decimal TotalAmount,
    decimal PendingAmount,
    decimal ApprovedAmount,
    int PendingCount
);

public record ExpenseDto(
    int Id,
    string ReferenceNumber,
    DateTime Date,
    string CategoryName,
    string? CategoryColor,
    string Description,
    decimal Amount,
    string? CurrencyCode,
    decimal AmountBase,
    string PaymentMethod,
    string? VendorName,
    ExpenseStatus Status,
    string? ApprovedBy,
    DateTime? ApprovedAt,
    bool IsRecurring
);

public class GetExpensesQueryHandler : IRequestHandler<GetExpensesQuery, ExpenseListResult>
{
    private readonly AppDbContext _context;

    public GetExpensesQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ExpenseListResult> Handle(GetExpensesQuery request, CancellationToken ct)
    {
        var query = _context.Expenses
            .AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.Currency)
            .AsQueryable();

        if (request.DateFrom.HasValue)
            query = query.Where(e => e.Date >= request.DateFrom.Value);

        if (request.DateTo.HasValue)
            query = query.Where(e => e.Date <= request.DateTo.Value);

        if (request.CategoryId.HasValue)
            query = query.Where(e => e.CategoryId == request.CategoryId.Value);

        if (request.Status.HasValue)
            query = query.Where(e => e.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(e =>
                e.Description.ToLower().Contains(term) ||
                e.ReferenceNumber.ToLower().Contains(term) ||
                (e.VendorName != null && e.VendorName.ToLower().Contains(term)));
        }

        var expenses = await query
            .OrderByDescending(e => e.Date)
            .ThenByDescending(e => e.Id)
            .ToListAsync(ct);

        var items = expenses.Select(e => new ExpenseDto(
            e.Id,
            e.ReferenceNumber,
            e.Date,
            e.Category?.Name ?? "Unknown",
            e.Category?.Color,
            e.Description,
            e.Amount,
            e.Currency?.Code,
            e.AmountBase,
            e.PaymentMethod.ToString(),
            e.VendorName,
            e.Status,
            e.ApprovedBy,
            e.ApprovedAt,
            e.IsRecurring
        )).ToList();

        var totalAmount = expenses.Sum(e => e.AmountBase);
        var pendingAmount = expenses.Where(e => e.Status == ExpenseStatus.Pending).Sum(e => e.AmountBase);
        var approvedAmount = expenses.Where(e => e.Status == ExpenseStatus.Approved).Sum(e => e.AmountBase);
        var pendingCount = expenses.Count(e => e.Status == ExpenseStatus.Pending);

        return new ExpenseListResult(items, totalAmount, pendingAmount, approvedAmount, pendingCount);
    }
}
