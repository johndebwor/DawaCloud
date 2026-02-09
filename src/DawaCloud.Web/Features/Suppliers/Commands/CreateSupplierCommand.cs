using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Suppliers.Commands;

public record CreateSupplierCommand(
    string Name,
    string? Email,
    string? Phone,
    string? Address,
    string? City,
    string? Country,
    string? TaxId,
    bool EmailNotificationsEnabled,
    bool WhatsAppNotificationsEnabled,
    string? Notes,
    List<CreateSupplierContactDto>? Contacts
) : IRequest<CreateSupplierResult>;

public record CreateSupplierContactDto(
    string Name,
    string? Email,
    string? Phone,
    string? Position,
    bool IsPrimary
);

public record CreateSupplierResult(
    bool Success,
    string Message,
    int? SupplierId = null
);

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, CreateSupplierResult>
{
    private readonly AppDbContext _context;

    public CreateSupplierCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CreateSupplierResult> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Generate supplier code
            var code = await GenerateSupplierCodeAsync(cancellationToken);

            var supplier = new Supplier
            {
                Code = code,
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                City = request.City,
                Country = request.Country ?? "South Sudan",
                TaxId = request.TaxId,
                EmailNotificationsEnabled = request.EmailNotificationsEnabled,
                WhatsAppNotificationsEnabled = request.WhatsAppNotificationsEnabled,
                IsActive = true,
                Notes = request.Notes
            };

            // Add contacts
            if (request.Contacts != null)
            {
                foreach (var contactDto in request.Contacts)
                {
                    var contact = new SupplierContact
                    {
                        Name = contactDto.Name,
                        Email = contactDto.Email,
                        Phone = contactDto.Phone,
                        Position = contactDto.Position,
                        IsPrimary = contactDto.IsPrimary
                    };
                    supplier.Contacts.Add(contact);
                }
            }

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreateSupplierResult(
                true,
                $"Supplier '{supplier.Name}' created successfully with code {code}",
                supplier.Id
            );
        }
        catch (Exception ex)
        {
            return new CreateSupplierResult(
                false,
                $"Error creating supplier: {ex.Message}"
            );
        }
    }

    private async Task<string> GenerateSupplierCodeAsync(CancellationToken cancellationToken)
    {
        var lastSupplier = await _context.Suppliers
            .OrderByDescending(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var nextNumber = lastSupplier != null ? lastSupplier.Id + 1 : 1;
        return $"SUP-{nextNumber:D4}";
    }
}
