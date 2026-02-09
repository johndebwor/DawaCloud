using DawaFlow.Web.Data;
using DawaFlow.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Wholesale.Commands;

public record ConvertQuotationToSaleCommand(
    int QuotationId,
    string? DeliveryAddress = null
) : IRequest<Result<int>>; // Returns Sale ID

public class ConvertQuotationToSaleCommandHandler : IRequestHandler<ConvertQuotationToSaleCommand, Result<int>>
{
    private readonly AppDbContext _context;

    public ConvertQuotationToSaleCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(ConvertQuotationToSaleCommand request, CancellationToken ct)
    {
        // Load quotation with items
        var quotation = await _context.Quotations
            .Include(q => q.Customer)
            .Include(q => q.Items)
                .ThenInclude(i => i.Drug)
            .FirstOrDefaultAsync(q => q.Id == request.QuotationId && !q.IsDeleted, ct);

        if (quotation == null)
            return Result<int>.Fail("Quotation not found");

        if (quotation.Status == QuotationStatus.Converted)
            return Result<int>.Fail("Quotation has already been converted to a sale");

        if (quotation.Status == QuotationStatus.Expired)
            return Result<int>.Fail("Cannot convert expired quotation");

        if (quotation.Status == QuotationStatus.Rejected)
            return Result<int>.Fail("Cannot convert rejected quotation");

        if (quotation.ValidUntil < DateTime.Now)
            return Result<int>.Fail("Quotation has expired");

        // Check stock availability for all items
        var saleItems = new List<WholesaleSaleItem>();

        foreach (var quotItem in quotation.Items)
        {
            // Get available batches with FEFO (First Expiry, First Out)
            var batches = await _context.Batches
                .Where(b => b.DrugId == quotItem.DrugId
                    && b.Status == BatchStatus.Active
                    && b.CurrentQuantity > 0
                    && !b.IsDeleted)
                .OrderBy(b => b.ExpiryDate)
                .ToListAsync(ct);

            var totalAvailable = batches.Sum(b => b.CurrentQuantity);

            if (totalAvailable < quotItem.Quantity)
                return Result<int>.Fail($"Insufficient stock for {quotItem.Drug.Name}. Available: {totalAvailable}, Required: {quotItem.Quantity}");

            // Allocate quantity across batches (FEFO)
            int remainingQuantity = quotItem.Quantity;
            foreach (var batch in batches)
            {
                if (remainingQuantity == 0) break;

                var quantityFromBatch = Math.Min(remainingQuantity, batch.CurrentQuantity);

                saleItems.Add(new WholesaleSaleItem
                {
                    DrugId = quotItem.DrugId,
                    BatchId = batch.Id,
                    Quantity = quantityFromBatch,
                    UnitPrice = quotItem.UnitPrice,
                    DiscountPercent = quotItem.DiscountPercent,
                    TaxRate = quotItem.TaxRate,
                    LineTotal = quantityFromBatch * quotItem.UnitPrice * (1 - quotItem.DiscountPercent / 100),
                    CreatedAt = DateTime.UtcNow
                });

                // Reserve stock
                batch.ReservedQuantity += quantityFromBatch;
                remainingQuantity -= quantityFromBatch;
            }
        }

        // Check credit limit
        var newBalance = quotation.Customer.CurrentBalance + quotation.TotalAmount;
        if (newBalance > quotation.Customer.CreditLimit)
        {
            return Result<int>.Fail(
                $"Credit limit exceeded. Current balance: SSP {quotation.Customer.CurrentBalance:N2}, " +
                $"Credit limit: SSP {quotation.Customer.CreditLimit:N2}, " +
                $"This sale: SSP {quotation.TotalAmount:N2}");
        }

        // Generate invoice number
        var lastSale = await _context.WholesaleSales
            .OrderByDescending(s => s.Id)
            .FirstOrDefaultAsync(ct);

        var nextNumber = (lastSale?.Id ?? 0) + 1;
        var invoiceNumber = $"INV-{DateTime.Now:yyyyMM}-{nextNumber:D5}";

        // Create wholesale sale
        var sale = new WholesaleSale
        {
            InvoiceNumber = invoiceNumber,
            CustomerId = quotation.CustomerId,
            SaleDate = DateTime.Now,
            SubTotal = quotation.SubTotal,
            TaxAmount = quotation.TaxAmount,
            DiscountAmount = quotation.DiscountAmount,
            TotalAmount = quotation.TotalAmount,
            PaidAmount = 0,
            PaymentStatus = PaymentStatus.Pending,
            Status = SaleStatus.Confirmed,
            DeliveryAddress = request.DeliveryAddress,
            Notes = $"Converted from Quotation {quotation.QuotationNumber}",
            Items = saleItems,
            CreatedAt = DateTime.UtcNow
        };

        _context.WholesaleSales.Add(sale);

        // Update customer balance
        quotation.Customer.CurrentBalance = newBalance;

        // Update quotation status
        quotation.Status = QuotationStatus.Converted;
        quotation.ConvertedToSaleId = sale.Id;

        await _context.SaveChangesAsync(ct);

        return Result<int>.Ok(sale.Id);
    }
}
