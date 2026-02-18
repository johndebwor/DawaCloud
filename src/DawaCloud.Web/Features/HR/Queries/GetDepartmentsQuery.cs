using DawaCloud.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.HR.Queries;

public record GetDepartmentsQuery(string? SearchTerm = null) : IRequest<List<DepartmentDto>>;

public class DepartmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? HeadEmployeeName { get; set; }
    public int EmployeeCount { get; set; }
    public bool IsActive { get; set; }
}

public class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, List<DepartmentDto>>
{
    private readonly AppDbContext _context;

    public GetDepartmentsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DepartmentDto>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Departments
            .Include(d => d.Employees)
            .Include(d => d.HeadEmployee)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(d =>
                d.Name.Contains(request.SearchTerm) ||
                (d.Description != null && d.Description.Contains(request.SearchTerm)));
        }

        return await query
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                HeadEmployeeName = d.HeadEmployee != null
                    ? d.HeadEmployee.FirstName + " " + d.HeadEmployee.LastName
                    : null,
                EmployeeCount = d.Employees.Count,
                IsActive = d.IsActive
            })
            .ToListAsync(cancellationToken);
    }
}
