using DawaFlow.Web.Data;
using DawaFlow.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Inventory.Queries;

public class GetBatchesForAdjustmentQueryHandler : IRequestHandler<GetBatchesForAdjustmentQuery, List<BatchForAdjustmentDto>>
{
    private readonly AppDbContext _context;

    public GetBatchesForAdjustmentQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BatchForAdjustmentDto>> Handle(GetBatchesForAdjustmentQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Batches
            .Include(b => b.Drug)
            .Where(b => b.Status == BatchStatus.Active || b.Status == BatchStatus.Depleted)
            .AsQueryable();

        if (request.DrugId.HasValue)
        {
            query = query.Where(b => b.DrugId == request.DrugId.Value);
        }

        var batches = await query
            .OrderBy(b => b.ExpiryDate)
            .ThenBy(b => b.Drug.Name)
            .Select(b => new BatchForAdjustmentDto
            {
                Id = b.Id,
                DrugId = b.DrugId,
                DrugName = b.Drug.Name,
                BatchNumber = b.BatchNumber,
                ExpiryDate = b.ExpiryDate,
                CurrentQuantity = b.CurrentQuantity,
                ReservedQuantity = b.ReservedQuantity,
                Status = b.Status.ToString()
            })
            .ToListAsync(cancellationToken);

        return batches;
    }
}
