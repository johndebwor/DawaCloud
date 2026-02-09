using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Procurement.Commands;

public record CreateDrugRequestCommand(
    int SupplierId,
    string? Notes,
    List<DrugRequestItemDto> Items
) : IRequest<CreateDrugRequestResult>;

public record DrugRequestItemDto(
    int DrugId,
    int RequestedQuantity,
    decimal UnitPrice
);

public record CreateDrugRequestResult(
    bool Success,
    string Message,
    int? RequestId = null
);

public class CreateDrugRequestCommandHandler : IRequestHandler<CreateDrugRequestCommand, CreateDrugRequestResult>
{
    private readonly AppDbContext _context;

    public CreateDrugRequestCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CreateDrugRequestResult> Handle(CreateDrugRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Generate request number
            var requestNumber = await GenerateRequestNumberAsync(cancellationToken);

            var drugRequest = new DrugRequest
            {
                RequestNumber = requestNumber,
                RequestDate = DateTime.UtcNow,
                SupplierId = request.SupplierId,
                Status = DrugRequestStatus.Draft,
                Notes = request.Notes
            };

            // Add items
            decimal totalAmount = 0;
            foreach (var itemDto in request.Items)
            {
                var totalPrice = itemDto.RequestedQuantity * itemDto.UnitPrice;
                totalAmount += totalPrice;

                var item = new DrugRequestItem
                {
                    DrugId = itemDto.DrugId,
                    RequestedQuantity = itemDto.RequestedQuantity,
                    UnitPrice = itemDto.UnitPrice,
                    TotalPrice = totalPrice
                };
                drugRequest.Items.Add(item);
            }

            drugRequest.TotalAmount = totalAmount;

            _context.DrugRequests.Add(drugRequest);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreateDrugRequestResult(
                true,
                $"Drug request {requestNumber} created successfully with {request.Items.Count} items",
                drugRequest.Id
            );
        }
        catch (Exception ex)
        {
            return new CreateDrugRequestResult(
                false,
                $"Error creating drug request: {ex.Message}"
            );
        }
    }

    private async Task<string> GenerateRequestNumberAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow;
        var prefix = $"DR-{today:yyyyMM}";

        var lastRequest = await _context.DrugRequests
            .Where(dr => dr.RequestNumber.StartsWith(prefix))
            .OrderByDescending(dr => dr.Id)
            .FirstOrDefaultAsync(cancellationToken);

        int sequence = 1;
        if (lastRequest != null)
        {
            var lastSequence = lastRequest.RequestNumber.Split('-').LastOrDefault();
            if (int.TryParse(lastSequence, out int parsed))
            {
                sequence = parsed + 1;
            }
        }

        return $"{prefix}-{sequence:D4}";
    }
}
