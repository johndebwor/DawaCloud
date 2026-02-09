using DawaFlow.Web.Data;
using DawaFlow.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Wholesale.Commands;

public record CreateSaleCommand(
    int CustomerId,
    DateTime SaleDate,
    List<SaleItemDto> Items,
    string? Notes,
    string? DeliveryAddress
) : IRequest<Result<int>>;

public record SaleItemDto(
    int DrugId,
    int BatchId,
    int Quantity,
    decimal UnitPrice
);

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Result<int>>
{
    private readonly AppDbContext _context;

    public CreateSaleCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateSaleCommand request, CancellationToken ct)
    {
        // Validate customer
        var customer = await _context.WholesaleCustomers
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId && !c.IsDeleted, ct);

        if (customer == null)
            return Result<int>.Fail("Customer not found");

        // Calculate totals
        decimal subTotal = 0;
        var saleItems = new List<WholesaleSaleItem>();

        foreach (var item in request.Items)
        {
            var batch = await _context.Batches
                .Include(b => b.Drug)
                .FirstOrDefaultAsync(b => b.Id == item.BatchId && !b.IsDeleted, ct);

            if (batch == null)
                return Result<int>.Fail($"Batch {item.BatchId} not found");

            if (batch.CurrentQuantity < item.Quantity)
                return Result<int>.Fail($"Insufficient stock for {batch.Drug.Name}");

            var lineTotal = item.Quantity * item.UnitPrice;
            subTotal += lineTotal;

            saleItems.Add(new WholesaleSaleItem
            {
                DrugId = item.DrugId,
                BatchId = item.BatchId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = lineTotal,
                CreatedAt = DateTime.UtcNow
            });

            // Reserve stock
            batch.ReservedQuantity += item.Quantity;
        }

        // Calculate tax (16%)
        var taxAmount = subTotal * 0.16m;
        var totalAmount = subTotal + taxAmount;

        // Check credit limit
        var newBalance = customer.CurrentBalance + totalAmount;
        if (newBalance > customer.CreditLimit)
        {
            return Result<int>.Fail($"Credit limit exceeded. Current balance: SSP {customer.CurrentBalance:N2}, Credit limit: SSP {customer.CreditLimit:N2}");
        }

        // Generate invoice number
        var lastSale = await _context.WholesaleSales
            .OrderByDescending(s => s.Id)
            .FirstOrDefaultAsync(ct);

        var nextNumber = (lastSale?.Id ?? 0) + 1;
        var invoiceNumber = $"INV-{DateTime.Now:yyyyMM}-{nextNumber:D5}";

        // Create sale
        var sale = new WholesaleSale
        {
            InvoiceNumber = invoiceNumber,
            CustomerId = request.CustomerId,
            SaleDate = request.SaleDate,
            SubTotal = subTotal,
            TaxAmount = taxAmount,
            DiscountAmount = 0,
            TotalAmount = totalAmount,
            PaidAmount = 0,
            PaymentStatus = PaymentStatus.Pending,
            Status = SaleStatus.Draft,
            DeliveryAddress = request.DeliveryAddress,
            Notes = request.Notes,
            Items = saleItems,
            CreatedAt = DateTime.UtcNow
        };

        _context.WholesaleSales.Add(sale);

        // Update customer balance
        customer.CurrentBalance = newBalance;

        await _context.SaveChangesAsync(ct);

        return Result<int>.Ok(sale.Id);
    }
}
