using DawaCloud.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Expenses.Queries;

public record GetExpenseCategoriesQuery : IRequest<List<ExpenseCategoryDto>>;

public record ExpenseCategoryDto(
    int Id,
    string Name,
    string? Description,
    decimal? BudgetLimit,
    string? Color,
    string? Icon,
    bool IsActive,
    int ExpenseCount
);

public class GetExpenseCategoriesQueryHandler : IRequestHandler<GetExpenseCategoriesQuery, List<ExpenseCategoryDto>>
{
    private readonly AppDbContext _context;

    public GetExpenseCategoriesQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ExpenseCategoryDto>> Handle(GetExpenseCategoriesQuery request, CancellationToken ct)
    {
        return await _context.ExpenseCategories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new ExpenseCategoryDto(
                c.Id,
                c.Name,
                c.Description,
                c.BudgetLimit,
                c.Color,
                c.Icon,
                c.IsActive,
                c.Expenses.Count(e => !e.IsDeleted)
            ))
            .ToListAsync(ct);
    }
}
