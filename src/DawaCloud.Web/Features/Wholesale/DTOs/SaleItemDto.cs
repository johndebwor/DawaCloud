namespace DawaCloud.Web.Features.Wholesale.DTOs;

public class SaleItemDto
{
    public int DrugId { get; set; }
    public string DrugCode { get; set; } = string.Empty;
    public string DrugName { get; set; } = string.Empty;
    public int BatchId { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal LineTotal => Quantity * UnitPrice * (1 - DiscountPercent / 100);
}
