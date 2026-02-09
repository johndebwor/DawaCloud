using DawaFlow.Web.Data;
using DawaFlow.Web.Data.Entities;
using MediatR;

namespace DawaFlow.Web.Features.Settings.Commands;

// Create Tax Category
public record CreateTaxCategoryCommand(
    string Name,
    decimal Rate,
    string? Description,
    bool IsDefault
) : IRequest<int>;

public class CreateTaxCategoryCommandHandler : IRequestHandler<CreateTaxCategoryCommand, int>
{
    private readonly AppDbContext _context;

    public CreateTaxCategoryCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTaxCategoryCommand request, CancellationToken ct)
    {
        var taxCategory = new TaxCategory
        {
            Name = request.Name,
            Rate = request.Rate,
            Description = request.Description,
            IsDefault = request.IsDefault,
            IsActive = true
        };

        _context.TaxCategories.Add(taxCategory);
        await _context.SaveChangesAsync(ct);

        return taxCategory.Id;
    }
}

// Update Tax Category
public record UpdateTaxCategoryCommand(
    int Id,
    string Name,
    decimal Rate,
    string? Description,
    bool IsDefault,
    bool IsActive
) : IRequest;

public class UpdateTaxCategoryCommandHandler : IRequestHandler<UpdateTaxCategoryCommand>
{
    private readonly AppDbContext _context;

    public UpdateTaxCategoryCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateTaxCategoryCommand request, CancellationToken ct)
    {
        var taxCategory = await _context.TaxCategories.FindAsync(new object[] { request.Id }, ct);

        if (taxCategory != null)
        {
            taxCategory.Name = request.Name;
            taxCategory.Rate = request.Rate;
            taxCategory.Description = request.Description;
            taxCategory.IsDefault = request.IsDefault;
            taxCategory.IsActive = request.IsActive;

            await _context.SaveChangesAsync(ct);
        }
    }
}

// Delete Tax Category
public record DeleteTaxCategoryCommand(int Id) : IRequest;

public class DeleteTaxCategoryCommandHandler : IRequestHandler<DeleteTaxCategoryCommand>
{
    private readonly AppDbContext _context;

    public DeleteTaxCategoryCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteTaxCategoryCommand request, CancellationToken ct)
    {
        var taxCategory = await _context.TaxCategories.FindAsync(new object[] { request.Id }, ct);

        if (taxCategory != null)
        {
            _context.TaxCategories.Remove(taxCategory);
            await _context.SaveChangesAsync(ct);
        }
    }
}
