using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Wholesale.Commands;

public record CreateQuotationCommand(
    int CustomerId,
    DateTime ValidUntil,
    List<QuotationItemDto> Items,
    string? Notes
) : IRequest<Result<int>>;

public record QuotationItemDto(
    int DrugId,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountPercent = 0
);

public class CreateQuotationCommandHandler : IRequestHandler<CreateQuotationCommand, Result<int>>
{
    private readonly AppDbContext _context;

    public CreateQuotationCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateQuotationCommand request, CancellationToken ct)
    {
        // Validate customer
        var customer = await _context.WholesaleCustomers
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId && !c.IsDeleted, ct);

        if (customer == null)
            return Result<int>.Fail("Customer not found");

        if (request.ValidUntil <= DateTime.Now)
            return Result<int>.Fail("Valid until date must be in the future");

        if (request.Items.Count == 0)
            return Result<int>.Fail("Quotation must have at least one item");

        // Calculate totals
        decimal subTotal = 0;
        var quotationItems = new List<QuotationItem>();

        foreach (var item in request.Items)
        {
            var drug = await _context.Drugs
                .FirstOrDefaultAsync(d => d.Id == item.DrugId && !d.IsDeleted, ct);

            if (drug == null)
                return Result<int>.Fail($"Drug with ID {item.DrugId} not found");

            var lineSubtotal = item.Quantity * item.UnitPrice;
            var discountAmount = lineSubtotal * (item.DiscountPercent / 100);
            var lineAfterDiscount = lineSubtotal - discountAmount;
            var lineTotal = lineAfterDiscount; // Tax will be applied to total

            subTotal += lineSubtotal;

            quotationItems.Add(new QuotationItem
            {
                DrugId = item.DrugId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                DiscountPercent = item.DiscountPercent,
                TaxRate = 16, // 16% VAT
                LineTotal = lineTotal,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Calculate totals
        var totalDiscount = quotationItems.Sum(i => i.Quantity * i.UnitPrice * i.DiscountPercent / 100);
        var subtotalAfterDiscount = subTotal - totalDiscount;
        var taxAmount = subtotalAfterDiscount * 0.16m; // 16% VAT
        var totalAmount = subtotalAfterDiscount + taxAmount;

        // Generate quotation number
        var lastQuotation = await _context.Quotations
            .OrderByDescending(q => q.Id)
            .FirstOrDefaultAsync(ct);

        var nextNumber = (lastQuotation?.Id ?? 0) + 1;
        var quotationNumber = $"QUO-{DateTime.Now:yyyyMM}-{nextNumber:D5}";

        // Create quotation
        var quotation = new Quotation
        {
            QuotationNumber = quotationNumber,
            CustomerId = request.CustomerId,
            QuotationDate = DateTime.Now,
            ValidUntil = request.ValidUntil,
            SubTotal = subTotal,
            TaxAmount = taxAmount,
            DiscountAmount = totalDiscount,
            TotalAmount = totalAmount,
            Status = QuotationStatus.Draft,
            Notes = request.Notes,
            Items = quotationItems,
            CreatedAt = DateTime.UtcNow
        };

        _context.Quotations.Add(quotation);
        await _context.SaveChangesAsync(ct);

        return Result<int>.Ok(quotation.Id);
    }
}
