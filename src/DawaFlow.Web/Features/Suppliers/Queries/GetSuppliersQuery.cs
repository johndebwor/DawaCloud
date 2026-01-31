using DawaFlow.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Suppliers.Queries;

public record GetSuppliersQuery(
    string? SearchTerm = null,
    bool? IsActive = null
) : IRequest<List<SupplierDto>>;

public class SupplierDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool EmailNotificationsEnabled { get; set; }
    public bool WhatsAppNotificationsEnabled { get; set; }
    public bool IsActive { get; set; }
    public int ContactCount { get; set; }
}

public class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, List<SupplierDto>>
{
    private readonly AppDbContext _context;

    public GetSuppliersQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SupplierDto>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Suppliers
            .Include(s => s.Contacts)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(s =>
                s.Name.Contains(request.SearchTerm) ||
                s.Code.Contains(request.SearchTerm) ||
                (s.Email != null && s.Email.Contains(request.SearchTerm)));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(s => s.IsActive == request.IsActive.Value);
        }

        var suppliers = await query
            .OrderBy(s => s.Name)
            .Select(s => new SupplierDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                Email = s.Email,
                Phone = s.Phone,
                City = s.City,
                Country = s.Country,
                EmailNotificationsEnabled = s.EmailNotificationsEnabled,
                WhatsAppNotificationsEnabled = s.WhatsAppNotificationsEnabled,
                IsActive = s.IsActive,
                ContactCount = s.Contacts.Count
            })
            .ToListAsync(cancellationToken);

        return suppliers;
    }
}
