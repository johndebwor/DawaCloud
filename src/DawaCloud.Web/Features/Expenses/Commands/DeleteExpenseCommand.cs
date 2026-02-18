using DawaCloud.Web.Data;
using DawaCloud.Web.Shared;
using MediatR;

namespace DawaCloud.Web.Features.Expenses.Commands;

public record DeleteExpenseCommand(int Id) : IRequest<Result<bool>>;

public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand, Result<bool>>
{
    private readonly AppDbContext _context;

    public DeleteExpenseCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteExpenseCommand request, CancellationToken ct)
    {
        var expense = await _context.Expenses.FindAsync([request.Id], ct);
        if (expense == null)
            return Result<bool>.Fail("Expense not found");

        expense.IsDeleted = true;
        expense.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}
