using DawaFlow.Web.Data;
using DawaFlow.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Procurement.Queries;

public record GetDrugRequestsQuery(
    DrugRequestStatus? Status = null,
    int? SupplierId = null
) : IRequest<List<DrugRequestSummaryDto>>;

public class DrugRequestSummaryDto
{
    public int Id { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class GetDrugRequestsQueryHandler : IRequestHandler<GetDrugRequestsQuery, List<DrugRequestSummaryDto>>
{
    private readonly AppDbContext _context;

    public GetDrugRequestsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DrugRequestSummaryDto>> Handle(GetDrugRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.DrugRequests
            .Include(dr => dr.Supplier)
            .Include(dr => dr.Items)
            .AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(dr => dr.Status == request.Status.Value);
        }

        if (request.SupplierId.HasValue)
        {
            query = query.Where(dr => dr.SupplierId == request.SupplierId.Value);
        }

        var drugRequests = await query
            .OrderByDescending(dr => dr.RequestDate)
            .Select(dr => new DrugRequestSummaryDto
            {
                Id = dr.Id,
                RequestNumber = dr.RequestNumber,
                RequestDate = dr.RequestDate,
                SupplierName = dr.Supplier.Name,
                Status = dr.Status.ToString(),
                ItemCount = dr.Items.Count,
                TotalAmount = dr.TotalAmount,
                ApprovedBy = dr.ApprovedBy,
                ApprovedAt = dr.ApprovedAt,
                ExpectedDeliveryDate = dr.ExpectedDeliveryDate,
                CreatedBy = dr.CreatedBy ?? "System"
            })
            .ToListAsync(cancellationToken);

        return drugRequests;
    }
}
