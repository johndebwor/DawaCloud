using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.HR.Commands;

public record CreateEmployeeCommand(
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    Gender? Gender,
    DateTime? DateOfBirth,
    string? NationalId,
    string? Address,
    string? City,
    int DepartmentId,
    string Position,
    EmploymentType EmploymentType,
    DateTime HireDate,
    decimal BasicSalary,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? EmergencyContactRelation,
    string? Notes
) : IRequest<Result<int>>;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Result<int>>
{
    private readonly AppDbContext _context;

    public CreateEmployeeCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var code = await GenerateEmployeeCodeAsync(cancellationToken);

            var employee = new Employee
            {
                EmployeeCode = code,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                NationalId = request.NationalId,
                Address = request.Address,
                City = request.City,
                DepartmentId = request.DepartmentId,
                Position = request.Position,
                EmploymentType = request.EmploymentType,
                Status = EmploymentStatus.Active,
                HireDate = request.HireDate,
                BasicSalary = request.BasicSalary,
                EmergencyContactName = request.EmergencyContactName,
                EmergencyContactPhone = request.EmergencyContactPhone,
                EmergencyContactRelation = request.EmergencyContactRelation,
                Notes = request.Notes
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Ok(employee.Id);
        }
        catch (Exception ex)
        {
            return Result<int>.Fail($"Error creating employee: {ex.Message}");
        }
    }

    private async Task<string> GenerateEmployeeCodeAsync(CancellationToken cancellationToken)
    {
        var lastEmployee = await _context.Employees
            .IgnoreQueryFilters()
            .OrderByDescending(e => e.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var nextNumber = lastEmployee != null ? lastEmployee.Id + 1 : 1;
        return $"EMP-{nextNumber:D4}";
    }
}
