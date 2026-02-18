using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Shared;
using MediatR;

namespace DawaCloud.Web.Features.Expenses.Commands;

public record ApproveExpenseCommand(int Id, bool Approve, string ApprovedBy) : IRequest<Result<bool>>;

public class ApproveExpenseCommandHandler : IRequestHandler<ApproveExpenseCommand, Result<bool>>
{
    private readonly AppDbContext _context;

    public ApproveExpenseCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(ApproveExpenseCommand request, CancellationToken ct)
    {
        var expense = await _context.Expenses.FindAsync([request.Id], ct);
        if (expense == null)
            return Result<bool>.Fail("Expense not found");

        if (expense.Status != ExpenseStatus.Pending)
            return Result<bool>.Fail("Only pending expenses can be approved or rejected");

        expense.Status = request.Approve ? ExpenseStatus.Approved : ExpenseStatus.Rejected;
        expense.ApprovedBy = request.ApprovedBy;
        expense.ApprovedAt = DateTime.UtcNow;
        expense.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}
