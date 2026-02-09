using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Inventory.Queries;

public class GetStockMovementsQueryHandler : IRequestHandler<GetStockMovementsQuery, StockMovementsResult>
{
    private readonly AppDbContext _context;

    public GetStockMovementsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StockMovementsResult> Handle(GetStockMovementsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.StockMovements
            .Include(sm => sm.Drug)
            .Include(sm => sm.Batch)
            .AsQueryable();

        // Apply filters
        if (request.DrugId.HasValue)
        {
            query = query.Where(sm => sm.DrugId == request.DrugId.Value);
        }

        if (request.BatchId.HasValue)
        {
            query = query.Where(sm => sm.BatchId == request.BatchId.Value);
        }

        if (request.Type.HasValue)
        {
            query = query.Where(sm => sm.Type == request.Type.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(sm => sm.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(sm => sm.CreatedAt <= request.ToDate.Value);
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination and ordering
        var movements = await query
            .OrderByDescending(sm => sm.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(sm => new StockMovementDto
            {
                Id = sm.Id,
                DrugId = sm.DrugId,
                DrugName = sm.Drug.Name,
                DrugCode = sm.Drug.Code,
                BatchId = sm.BatchId,
                BatchNumber = sm.Batch.BatchNumber,
                Type = sm.Type.ToString(),
                Quantity = sm.Quantity,
                BalanceBefore = sm.BalanceBefore,
                BalanceAfter = sm.BalanceAfter,
                Reference = sm.Reference,
                Reason = sm.Reason,
                Notes = sm.Notes,
                CreatedAt = sm.CreatedAt,
                CreatedBy = sm.CreatedBy ?? "System"
            })
            .ToListAsync(cancellationToken);

        return new StockMovementsResult(movements, totalCount);
    }
}
