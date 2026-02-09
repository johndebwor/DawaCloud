using DawaCloud.Web.Data.Entities;
using MediatR;

namespace DawaCloud.Web.Features.Inventory.Queries;

public record GetStockMovementsQuery(
    int? DrugId = null,
    int? BatchId = null,
    MovementType? Type = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int PageNumber = 1,
    int PageSize = 50
) : IRequest<StockMovementsResult>;

public record StockMovementsResult(
    List<StockMovementDto> Movements,
    int TotalCount
);

public class StockMovementDto
{
    public long Id { get; set; }
    public int DrugId { get; set; }
    public string DrugName { get; set; } = string.Empty;
    public string DrugCode { get; set; } = string.Empty;
    public int BatchId { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int BalanceBefore { get; set; }
    public int BalanceAfter { get; set; }
    public string? Reference { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}
