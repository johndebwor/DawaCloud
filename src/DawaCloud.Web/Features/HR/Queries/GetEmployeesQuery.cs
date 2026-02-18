using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.HR.Queries;

public record GetEmployeesQuery(
    string? SearchTerm = null,
    int? DepartmentId = null,
    EmploymentStatus? Status = null
) : IRequest<List<EmployeeDto>>;

public class EmployeeDto
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public EmploymentType EmploymentType { get; set; }
    public EmploymentStatus Status { get; set; }
    public DateTime HireDate { get; set; }
    public decimal BasicSalary { get; set; }
}

public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, List<EmployeeDto>>
{
    private readonly AppDbContext _context;

    public GetEmployeesQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Employees
            .Include(e => e.Department)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(e =>
                e.FirstName.Contains(request.SearchTerm) ||
                e.LastName.Contains(request.SearchTerm) ||
                e.EmployeeCode.Contains(request.SearchTerm) ||
                (e.Email != null && e.Email.Contains(request.SearchTerm)));
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == request.DepartmentId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(e => e.Status == request.Status.Value);
        }

        return await query
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                EmployeeCode = e.EmployeeCode,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                DepartmentName = e.Department.Name,
                Position = e.Position,
                EmploymentType = e.EmploymentType,
                Status = e.Status,
                HireDate = e.HireDate,
                BasicSalary = e.BasicSalary
            })
            .ToListAsync(cancellationToken);
    }
}
