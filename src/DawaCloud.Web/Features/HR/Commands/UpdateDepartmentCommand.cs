using DawaCloud.Web.Data;
using DawaCloud.Web.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.HR.Commands;

public record UpdateDepartmentCommand(
    int Id,
    string Name,
    string? Description,
    int? HeadEmployeeId,
    bool IsActive
) : IRequest<Result>;

public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Result>
{
    private readonly AppDbContext _context;

    public UpdateDepartmentCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (department == null)
                return Result.Fail("Department not found.");

            department.Name = request.Name;
            department.Description = request.Description;
            department.HeadEmployeeId = request.HeadEmployeeId;
            department.IsActive = request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error updating department: {ex.Message}");
        }
    }
}
