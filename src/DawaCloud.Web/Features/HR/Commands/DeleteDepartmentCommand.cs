using DawaCloud.Web.Data;
using DawaCloud.Web.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.HR.Commands;

public record DeleteDepartmentCommand(int Id) : IRequest<Result>;

public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Result>
{
    private readonly AppDbContext _context;

    public DeleteDepartmentCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var department = await _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (department == null)
                return Result.Fail("Department not found.");

            if (department.Employees.Any())
                return Result.Fail("Cannot delete department with active employees. Please reassign or remove employees first.");

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error deleting department: {ex.Message}");
        }
    }
}
