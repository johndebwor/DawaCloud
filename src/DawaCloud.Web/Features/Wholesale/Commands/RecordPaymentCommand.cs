using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Wholesale.Commands;

public record RecordPaymentCommand(
    int SaleId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    string? Reference,
    DateTime PaymentDate,
    string? Notes
) : IRequest<Result>;

public class RecordPaymentCommandHandler : IRequestHandler<RecordPaymentCommand, Result>
{
    private readonly AppDbContext _context;

    public RecordPaymentCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(RecordPaymentCommand request, CancellationToken ct)
    {
        var sale = await _context.WholesaleSales
            .Include(s => s.Customer)
            .Include(s => s.Items)
                .ThenInclude(i => i.Batch)
            .FirstOrDefaultAsync(s => s.Id == request.SaleId && !s.IsDeleted, ct);

        if (sale == null)
            return Result.Fail("Sale not found");

        if (request.Amount <= 0)
            return Result.Fail("Payment amount must be greater than zero");

        if (request.Amount > sale.BalanceAmount)
            return Result.Fail($"Payment amount exceeds balance. Balance: SSP {sale.BalanceAmount:N2}");

        // Create payment record
        var payment = new Payment
        {
            WholesaleSaleId = sale.Id,
            Amount = request.Amount,
            AmountBase = request.Amount,
            PaymentMethod = request.PaymentMethod,
            ReferenceNumber = request.Reference ?? string.Empty,
            PaymentDate = request.PaymentDate,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);

        // Update sale amounts
        sale.PaidAmount += request.Amount;

        // Update payment status
        if (sale.BalanceAmount == 0)
        {
            sale.PaymentStatus = PaymentStatus.Paid;
            sale.Status = SaleStatus.Completed;

            // Deduct from reserved stock
            foreach (var item in sale.Items)
            {
                if (item.Batch != null)
                {
                    item.Batch.ReservedQuantity -= item.Quantity;
                    item.Batch.CurrentQuantity -= item.Quantity;

                    // Check if batch is depleted
                    if (item.Batch.CurrentQuantity == 0)
                    {
                        item.Batch.Status = BatchStatus.Depleted;
                    }

                    // Create stock movement
                    var stockMovement = new StockMovement
                    {
                        DrugId = item.DrugId,
                        BatchId = item.BatchId.Value,
                        Type = MovementType.Sale,
                        Quantity = -item.Quantity,
                        BalanceBefore = item.Batch.CurrentQuantity + item.Quantity,
                        BalanceAfter = item.Batch.CurrentQuantity,
                        Reference = sale.InvoiceNumber,
                        Reason = "Wholesale Sale",
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.StockMovements.Add(stockMovement);
                }
            }
        }
        else if (sale.PaidAmount > 0)
        {
            sale.PaymentStatus = PaymentStatus.PartiallyPaid;
        }

        // Update customer balance
        sale.Customer.CurrentBalance -= request.Amount;

        await _context.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
