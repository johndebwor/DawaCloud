using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.BankAccounts.Commands;

// Create
public record CreateBankAccountCommand(
    string AccountName,
    string BankName,
    string? AccountNumber,
    string? BranchCode,
    int? CurrencyId,
    BankAccountType AccountType,
    decimal CurrentBalance,
    bool IsDefault
) : IRequest<Result<int>>;

public class CreateBankAccountCommandHandler : IRequestHandler<CreateBankAccountCommand, Result<int>>
{
    private readonly AppDbContext _context;
    public CreateBankAccountCommandHandler(AppDbContext context) => _context = context;

    public async Task<Result<int>> Handle(CreateBankAccountCommand request, CancellationToken ct)
    {
        if (request.IsDefault)
        {
            var existingDefaults = await _context.BankAccounts
                .Where(b => b.IsDefault)
                .ToListAsync(ct);
            foreach (var b in existingDefaults)
                b.IsDefault = false;
        }

        var account = new BankAccount
        {
            AccountName = request.AccountName,
            BankName = request.BankName,
            AccountNumber = request.AccountNumber,
            BranchCode = request.BranchCode,
            CurrencyId = request.CurrencyId,
            AccountType = request.AccountType,
            CurrentBalance = request.CurrentBalance,
            IsDefault = request.IsDefault,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.BankAccounts.Add(account);
        await _context.SaveChangesAsync(ct);

        return Result<int>.Ok(account.Id);
    }
}

// Update
public record UpdateBankAccountCommand(
    int Id,
    string AccountName,
    string BankName,
    string? AccountNumber,
    string? BranchCode,
    int? CurrencyId,
    BankAccountType AccountType,
    decimal CurrentBalance,
    bool IsDefault
) : IRequest<Result<bool>>;

public class UpdateBankAccountCommandHandler : IRequestHandler<UpdateBankAccountCommand, Result<bool>>
{
    private readonly AppDbContext _context;
    public UpdateBankAccountCommandHandler(AppDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(UpdateBankAccountCommand request, CancellationToken ct)
    {
        var account = await _context.BankAccounts.FindAsync([request.Id], ct);
        if (account == null)
            return Result<bool>.Fail("Bank account not found");

        if (request.IsDefault)
        {
            var existingDefaults = await _context.BankAccounts
                .Where(b => b.IsDefault && b.Id != request.Id)
                .ToListAsync(ct);
            foreach (var b in existingDefaults)
                b.IsDefault = false;
        }

        account.AccountName = request.AccountName;
        account.BankName = request.BankName;
        account.AccountNumber = request.AccountNumber;
        account.BranchCode = request.BranchCode;
        account.CurrencyId = request.CurrencyId;
        account.AccountType = request.AccountType;
        account.CurrentBalance = request.CurrentBalance;
        account.IsDefault = request.IsDefault;
        account.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}

// Delete
public record DeleteBankAccountCommand(int Id) : IRequest<Result<bool>>;

public class DeleteBankAccountCommandHandler : IRequestHandler<DeleteBankAccountCommand, Result<bool>>
{
    private readonly AppDbContext _context;
    public DeleteBankAccountCommandHandler(AppDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(DeleteBankAccountCommand request, CancellationToken ct)
    {
        var account = await _context.BankAccounts.FindAsync([request.Id], ct);
        if (account == null)
            return Result<bool>.Fail("Bank account not found");

        account.IsDeleted = true;
        account.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}
