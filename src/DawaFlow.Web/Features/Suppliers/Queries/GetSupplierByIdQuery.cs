using DawaFlow.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Suppliers.Queries;

public record GetSupplierByIdQuery(int Id) : IRequest<SupplierDetailsDto?>;

public class SupplierDetailsDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? TaxId { get; set; }
    public bool EmailNotificationsEnabled { get; set; }
    public bool WhatsAppNotificationsEnabled { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
    public List<SupplierContactDto> Contacts { get; set; } = new();
}

public class SupplierContactDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public bool IsPrimary { get; set; }
}

public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, SupplierDetailsDto?>
{
    private readonly AppDbContext _context;

    public GetSupplierByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SupplierDetailsDto?> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.Contacts)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (supplier == null)
        {
            return null;
        }

        return new SupplierDetailsDto
        {
            Id = supplier.Id,
            Code = supplier.Code,
            Name = supplier.Name,
            Email = supplier.Email,
            Phone = supplier.Phone,
            Address = supplier.Address,
            City = supplier.City,
            Country = supplier.Country,
            TaxId = supplier.TaxId,
            EmailNotificationsEnabled = supplier.EmailNotificationsEnabled,
            WhatsAppNotificationsEnabled = supplier.WhatsAppNotificationsEnabled,
            IsActive = supplier.IsActive,
            Notes = supplier.Notes,
            Contacts = supplier.Contacts.Select(c => new SupplierContactDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Position = c.Position,
                IsPrimary = c.IsPrimary
            }).ToList()
        };
    }
}
