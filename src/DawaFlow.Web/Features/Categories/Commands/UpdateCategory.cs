using DawaFlow.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Categories.Commands;

public record UpdateCategoryCommand(
    int Id,
    string Name,
    string? Description,
    int? ParentId,
    bool IsActive
) : IRequest;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly AppDbContext _context;

    public UpdateCategoryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.DrugCategories
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
            throw new InvalidOperationException("Category not found");

        // Prevent circular reference
        if (request.ParentId == request.Id)
            throw new InvalidOperationException("Category cannot be its own parent");

        // Validate parent change
        if (request.ParentId.HasValue && request.ParentId != category.ParentId)
        {
            var parent = await _context.DrugCategories
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(c => c.Id == request.ParentId.Value, cancellationToken);

            if (parent == null)
                throw new InvalidOperationException("Parent category not found");

            // Check if parent is a descendant (would create circular reference)
            if (await IsDescendant(request.Id, request.ParentId.Value, cancellationToken))
                throw new InvalidOperationException("Cannot set a descendant as parent (circular reference)");
        }

        category.Name = request.Name;
        category.Description = request.Description;
        category.ParentId = request.ParentId;
        category.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> IsDescendant(int ancestorId, int descendantId, CancellationToken cancellationToken)
    {
        var current = await _context.DrugCategories
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => c.Id == descendantId, cancellationToken);

        while (current?.ParentId != null)
        {
            if (current.ParentId == ancestorId)
                return true;

            current = current.Parent;
        }

        return false;
    }
}
