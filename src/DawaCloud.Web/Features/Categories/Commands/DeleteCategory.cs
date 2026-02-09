using DawaCloud.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Categories.Commands;

public record DeleteCategoryCommand(int Id) : IRequest;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly AppDbContext _context;

    public DeleteCategoryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.DrugCategories
            .Include(c => c.Children)
            .Include(c => c.Drugs)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
            throw new InvalidOperationException("Category not found");

        // Check if category has children
        if (category.Children.Any())
            throw new InvalidOperationException("Cannot delete category with subcategories. Delete or reassign subcategories first.");

        // Check if category has drugs
        if (category.Drugs.Any())
            throw new InvalidOperationException($"Cannot delete category with {category.Drugs.Count} drug(s). Reassign drugs first.");

        _context.DrugCategories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
