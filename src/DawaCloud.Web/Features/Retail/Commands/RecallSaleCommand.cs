using DawaCloud.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Retail.Commands;

public record RecallSaleCommand(int HeldSaleId) : IRequest<Result<HeldSaleDto>>;

public record HeldSaleDto(
    int Id,
    string Reference,
    string? CustomerName,
    decimal TotalAmount,
    DateTime HeldAt,
    string ItemsJson
);

public class RecallSaleCommandHandler : IRequestHandler<RecallSaleCommand, Result<HeldSaleDto>>
{
    private readonly AppDbContext _context;

    public RecallSaleCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<HeldSaleDto>> Handle(RecallSaleCommand request, CancellationToken ct)
    {
        var heldSale = await _context.HeldSales.FindAsync(new object[] { request.HeldSaleId }, ct);

        if (heldSale == null)
            return Result<HeldSaleDto>.Fail("Held sale not found");

        heldSale.IsRecalled = true;
        heldSale.RecalledAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);

        var dto = new HeldSaleDto(
            heldSale.Id,
            heldSale.Reference,
            heldSale.CustomerName,
            heldSale.TotalAmount,
            heldSale.HeldAt,
            heldSale.ItemsJson
        );

        return Result<HeldSaleDto>.Ok(dto);
    }
}
