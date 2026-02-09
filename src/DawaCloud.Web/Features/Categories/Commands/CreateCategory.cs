using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Categories.Commands;

public record CreateCategoryCommand(
    string Name,
    string? Description,
    int? ParentId
) : IRequest<int>;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, int>
{
    private readonly AppDbContext _context;

    public CreateCategoryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Validate parent if specified
        if (request.ParentId.HasValue)
        {
            var parent = await _context.DrugCategories
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(c => c.Id == request.ParentId.Value, cancellationToken);

            if (parent == null)
                throw new InvalidOperationException("Parent category not found");

            // Enforce max depth of 3 levels
            var depth = 1;
            var current = parent;
            while (current.ParentId != null)
            {
                depth++;
                current = current.Parent!;
            }

            if (depth >= 2)
                throw new InvalidOperationException("Maximum category depth (3 levels) exceeded");
        }

        var category = new DrugCategory
        {
            Name = request.Name,
            Description = request.Description,
            ParentId = request.ParentId,
            IsActive = true
        };

        _context.DrugCategories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
