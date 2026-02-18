using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.HR.Commands;

public record UpdateEmployeeCommand(
    int Id,
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
    EmploymentStatus Status,
    DateTime HireDate,
    DateTime? TerminationDate,
    decimal BasicSalary,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? EmergencyContactRelation,
    string? Notes
) : IRequest<Result>;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Result>
{
    private readonly AppDbContext _context;

    public UpdateEmployeeCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (employee == null)
                return Result.Fail("Employee not found.");

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Email = request.Email;
            employee.Phone = request.Phone;
            employee.Gender = request.Gender;
            employee.DateOfBirth = request.DateOfBirth;
            employee.NationalId = request.NationalId;
            employee.Address = request.Address;
            employee.City = request.City;
            employee.DepartmentId = request.DepartmentId;
            employee.Position = request.Position;
            employee.EmploymentType = request.EmploymentType;
            employee.Status = request.Status;
            employee.HireDate = request.HireDate;
            employee.TerminationDate = request.TerminationDate;
            employee.BasicSalary = request.BasicSalary;
            employee.EmergencyContactName = request.EmergencyContactName;
            employee.EmergencyContactPhone = request.EmergencyContactPhone;
            employee.EmergencyContactRelation = request.EmergencyContactRelation;
            employee.Notes = request.Notes;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error updating employee: {ex.Message}");
        }
    }
}
