using MediatR;

namespace DawaFlow.Web.Features.Inventory.Queries;

public record GetStockLevelsQuery(
    string? SearchTerm = null,
    string? Category = null,
    string? StockLevel = null
) : IRequest<List<StockLevelDto>>;

public class StockLevelDto
{
    public int DrugId { get; set; }
    public string DrugCode { get; set; } = string.Empty;
    public string DrugName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int ReorderLevel { get; set; }
    public string Unit { get; set; } = "Units";
    public int BatchCount { get; set; }
    public decimal TotalValue { get; set; }
    public string StockLevel { get; set; } = "Good";
}
