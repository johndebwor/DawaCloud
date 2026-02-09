using DawaCloud.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Settings.Queries;

public record GetTaxCategoriesQuery : IRequest<List<TaxCategoryDto>>;

public class GetTaxCategoriesQueryHandler : IRequestHandler<GetTaxCategoriesQuery, List<TaxCategoryDto>>
{
    private readonly AppDbContext _context;

    public GetTaxCategoriesQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaxCategoryDto>> Handle(GetTaxCategoriesQuery request, CancellationToken ct)
    {
        return await _context.TaxCategories
            .Where(tc => !tc.IsDeleted)
            .OrderBy(tc => tc.Name)
            .Select(tc => new TaxCategoryDto
            {
                Id = tc.Id,
                Name = tc.Name,
                Rate = tc.Rate,
                Description = tc.Description,
                IsDefault = tc.IsDefault,
                IsActive = tc.IsActive
            })
            .ToListAsync(ct);
    }
}

public class TaxCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}
