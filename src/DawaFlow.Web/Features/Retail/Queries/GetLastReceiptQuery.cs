using DawaFlow.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Retail.Queries;

public record GetLastReceiptQuery(string CashierId) : IRequest<Result<RetailReceiptDto>>;

public record RetailReceiptDto(
    int Id,
    string ReceiptNumber,
    DateTime SaleDate,
    string? CustomerName,
    decimal SubTotal,
    decimal TaxAmount,
    decimal DiscountAmount,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal ChangeAmount,
    string PaymentMethod,
    List<ReceiptItemDto> Items
);

public record ReceiptItemDto(
    string DrugName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal
);

public class GetLastReceiptQueryHandler : IRequestHandler<GetLastReceiptQuery, Result<RetailReceiptDto>>
{
    private readonly AppDbContext _context;

    public GetLastReceiptQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RetailReceiptDto>> Handle(GetLastReceiptQuery request, CancellationToken ct)
    {
        var lastSale = await _context.RetailSales
            .AsNoTracking()
            .Where(s => s.CashierId == request.CashierId)
            .OrderByDescending(s => s.SaleDate)
            .Include(s => s.Items)
                .ThenInclude(i => i.Drug)
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(ct);

        if (lastSale == null)
            return Result<RetailReceiptDto>.Fail("No recent receipts found");

        // Get payment method from the first payment, or default to "Cash"
        var paymentMethod = lastSale.Payments.FirstOrDefault()?.PaymentMethod.ToString() ?? "Cash";

        var dto = new RetailReceiptDto(
            lastSale.Id,
            lastSale.ReceiptNumber,
            lastSale.SaleDate,
            lastSale.CustomerName,
            lastSale.SubTotal,
            lastSale.TaxAmount,
            lastSale.DiscountAmount,
            lastSale.TotalAmount,
            lastSale.PaidAmount,
            lastSale.ChangeAmount,
            paymentMethod,
            lastSale.Items.Select(i => new ReceiptItemDto(
                i.Drug.Name,
                i.Quantity,
                i.UnitPrice,
                i.LineTotal
            )).ToList()
        );

        return Result<RetailReceiptDto>.Ok(dto);
    }
}
