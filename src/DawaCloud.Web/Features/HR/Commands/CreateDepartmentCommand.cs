using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Shared;
using MediatR;

namespace DawaCloud.Web.Features.HR.Commands;

public record CreateDepartmentCommand(
    string Name,
    string? Description,
    int? HeadEmployeeId
) : IRequest<Result<int>>;

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Result<int>>
{
    private readonly AppDbContext _context;

    public CreateDepartmentCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var department = new Department
            {
                Name = request.Name,
                Description = request.Description,
                HeadEmployeeId = request.HeadEmployeeId,
                IsActive = true
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Ok(department.Id);
        }
        catch (Exception ex)
        {
            return Result<int>.Fail($"Error creating department: {ex.Message}");
        }
    }
}
