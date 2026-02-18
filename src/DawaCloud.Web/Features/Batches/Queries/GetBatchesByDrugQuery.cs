using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Batches.Queries;

public record GetBatchesByDrugQuery(
    int DrugId,
    bool ActiveOnly = true
) : IRequest<List<BatchDto>>;

public record BatchDto(
    int Id,
    int DrugId,
    string BatchNumber,
    DateTime ManufactureDate,
    DateTime ExpiryDate,
    int InitialQuantity,
    int CurrentQuantity,
    int ReservedQuantity,
    decimal CostPrice,
    BatchStatus Status,
    int DaysToExpiry
);

public class GetBatchesByDrugQueryHandler : IRequestHandler<GetBatchesByDrugQuery, List<BatchDto>>
{
    private readonly AppDbContext _context;

    public GetBatchesByDrugQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BatchDto>> Handle(GetBatchesByDrugQuery request, CancellationToken ct)
    {
        var query = _context.Batches
            .AsNoTracking()
            .Where(b => b.DrugId == request.DrugId && !b.IsDeleted);

        if (request.ActiveOnly)
        {
            query = query.Where(b => b.Status == BatchStatus.Active && b.CurrentQuantity > 0);
        }

        var batches = await query
            .OrderBy(b => b.ExpiryDate) // FEFO - First Expiry, First Out
            .ThenBy(b => b.BatchNumber)
            .Select(b => new BatchDto(
                b.Id,
                b.DrugId,
                b.BatchNumber,
                b.ManufactureDate,
                b.ExpiryDate,
                b.InitialQuantity,
                b.CurrentQuantity,
                b.ReservedQuantity,
                b.CostPrice,
                b.Status,
                (int)(b.ExpiryDate - DateTime.UtcNow).TotalDays
            ))
            .ToListAsync(ct);

        return batches;
    }
}
