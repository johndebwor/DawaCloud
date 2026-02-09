using DawaCloud.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Categories.Queries;

public record GetCategoriesQuery : IRequest<List<CategoryDto>>;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly AppDbContext _context;

    public GetCategoriesHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _context.DrugCategories
            .Include(c => c.Children)
            .Include(c => c.Drugs)
            .Where(c => c.ParentId == null)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return categories.Select(c => MapToDto(c)).ToList();
    }

    private CategoryDto MapToDto(Data.Entities.DrugCategory category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentId = category.ParentId,
            IsActive = category.IsActive,
            DrugCount = category.Drugs.Count,
            Children = category.Children
                .OrderBy(c => c.Name)
                .Select(c => MapToDto(c))
                .ToList()
        };
    }
}

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public bool IsActive { get; set; }
    public int DrugCount { get; set; }
    public bool IsExpanded { get; set; } = true;
    public List<CategoryDto> Children { get; set; } = new();
}
