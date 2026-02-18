using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.BankAccounts.Queries;

public record GetBankAccountsQuery : IRequest<List<BankAccountDto>>;

public record BankAccountDto(
    int Id,
    string AccountName,
    string BankName,
    string? AccountNumber,
    string? BranchCode,
    string? CurrencyCode,
    BankAccountType AccountType,
    decimal CurrentBalance,
    bool IsDefault,
    bool IsActive
);

public class GetBankAccountsQueryHandler : IRequestHandler<GetBankAccountsQuery, List<BankAccountDto>>
{
    private readonly AppDbContext _context;
    public GetBankAccountsQueryHandler(AppDbContext context) => _context = context;

    public async Task<List<BankAccountDto>> Handle(GetBankAccountsQuery request, CancellationToken ct)
    {
        return await _context.BankAccounts
            .AsNoTracking()
            .Include(b => b.Currency)
            .Where(b => b.IsActive)
            .OrderByDescending(b => b.IsDefault)
            .ThenBy(b => b.AccountName)
            .Select(b => new BankAccountDto(
                b.Id,
                b.AccountName,
                b.BankName,
                b.AccountNumber,
                b.BranchCode,
                b.Currency != null ? b.Currency.Code : null,
                b.AccountType,
                b.CurrentBalance,
                b.IsDefault,
                b.IsActive
            ))
            .ToListAsync(ct);
    }
}
