using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Shared;
using MediatR;

namespace DawaCloud.Web.Features.Expenses.Commands;

// Create
public record CreateExpenseCategoryCommand(
    string Name,
    string? Description,
    decimal? BudgetLimit,
    string? Color,
    string? Icon
) : IRequest<Result<int>>;

public class CreateExpenseCategoryCommandHandler : IRequestHandler<CreateExpenseCategoryCommand, Result<int>>
{
    private readonly AppDbContext _context;
    public CreateExpenseCategoryCommandHandler(AppDbContext context) => _context = context;

    public async Task<Result<int>> Handle(CreateExpenseCategoryCommand request, CancellationToken ct)
    {
        var category = new ExpenseCategory
        {
            Name = request.Name,
            Description = request.Description,
            BudgetLimit = request.BudgetLimit,
            Color = request.Color,
            Icon = request.Icon,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.ExpenseCategories.Add(category);
        await _context.SaveChangesAsync(ct);

        return Result<int>.Ok(category.Id);
    }
}

// Update
public record UpdateExpenseCategoryCommand(
    int Id,
    string Name,
    string? Description,
    decimal? BudgetLimit,
    string? Color,
    string? Icon
) : IRequest<Result<bool>>;

public class UpdateExpenseCategoryCommandHandler : IRequestHandler<UpdateExpenseCategoryCommand, Result<bool>>
{
    private readonly AppDbContext _context;
    public UpdateExpenseCategoryCommandHandler(AppDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(UpdateExpenseCategoryCommand request, CancellationToken ct)
    {
        var category = await _context.ExpenseCategories.FindAsync([request.Id], ct);
        if (category == null)
            return Result<bool>.Fail("Category not found");

        category.Name = request.Name;
        category.Description = request.Description;
        category.BudgetLimit = request.BudgetLimit;
        category.Color = request.Color;
        category.Icon = request.Icon;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}

// Delete
public record DeleteExpenseCategoryCommand(int Id) : IRequest<Result<bool>>;

public class DeleteExpenseCategoryCommandHandler : IRequestHandler<DeleteExpenseCategoryCommand, Result<bool>>
{
    private readonly AppDbContext _context;
    public DeleteExpenseCategoryCommandHandler(AppDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(DeleteExpenseCategoryCommand request, CancellationToken ct)
    {
        var category = await _context.ExpenseCategories.FindAsync([request.Id], ct);
        if (category == null)
            return Result<bool>.Fail("Category not found");

        category.IsDeleted = true;
        category.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}
