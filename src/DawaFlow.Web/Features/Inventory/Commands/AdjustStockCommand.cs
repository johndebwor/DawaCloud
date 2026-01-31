using MediatR;

namespace DawaFlow.Web.Features.Inventory.Commands;

public record AdjustStockCommand(
    int BatchId,
    int QuantityChange,
    string Reason,
    string? Notes
) : IRequest<AdjustStockResult>;

public record AdjustStockResult(
    bool Success,
    string Message,
    long? MovementId = null
);
