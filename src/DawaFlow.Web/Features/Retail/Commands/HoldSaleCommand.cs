using DawaFlow.Web.Data;
using DawaFlow.Web.Data.Entities;
using MediatR;
using System.Text.Json;

namespace DawaFlow.Web.Features.Retail.Commands;

public record HoldSaleCommand(
    string? CustomerName,
    List<CartItemDto> Items,
    decimal TotalAmount,
    decimal SubTotal,
    decimal TaxAmount,
    decimal DiscountAmount,
    string CashierId
) : IRequest<Result<string>>;

public record CartItemDto(int ProductId, string Name, int Quantity, decimal UnitPrice);

public class HoldSaleCommandHandler : IRequestHandler<HoldSaleCommand, Result<string>>
{
    private readonly AppDbContext _context;

    public HoldSaleCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<string>> Handle(HoldSaleCommand request, CancellationToken ct)
    {
        var reference = $"HOLD-{DateTime.Now:yyyyMMddHHmmss}";

        var heldSale = new HeldSale
        {
            Reference = reference,
            CustomerName = request.CustomerName,
            CashierId = request.CashierId,
            HeldAt = DateTime.UtcNow,
            ItemsJson = JsonSerializer.Serialize(request.Items),
            TotalAmount = request.TotalAmount,
            IsRecalled = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.HeldSales.Add(heldSale);
        await _context.SaveChangesAsync(ct);

        return Result<string>.Ok(reference);
    }
}

public record Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public string? Error { get; init; }

    public static Result<T> Ok(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Fail(string error) => new() { IsSuccess = false, Error = error };
}
