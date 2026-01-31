using MediatR;

namespace DawaFlow.Web.Features.Inventory.Queries;

public record GetBatchesForAdjustmentQuery(int? DrugId = null) : IRequest<List<BatchForAdjustmentDto>>;

public class BatchForAdjustmentDto
{
    public int Id { get; set; }
    public int DrugId { get; set; }
    public string DrugName { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int CurrentQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public string DisplayText => $"{BatchNumber} - {DrugName} (Qty: {CurrentQuantity}, Exp: {ExpiryDate:dd MMM yyyy})";
}
