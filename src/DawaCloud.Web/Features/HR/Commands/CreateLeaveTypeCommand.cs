using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Shared;
using MediatR;

namespace DawaCloud.Web.Features.HR.Commands;

public record CreateLeaveTypeCommand(
    string Name,
    int DefaultDaysPerYear,
    bool IsPaid,
    string? Description
) : IRequest<Result<int>>;

public class CreateLeaveTypeCommandHandler : IRequestHandler<CreateLeaveTypeCommand, Result<int>>
{
    private readonly AppDbContext _context;

    public CreateLeaveTypeCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateLeaveTypeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var leaveType = new LeaveType
            {
                Name = request.Name,
                DefaultDaysPerYear = request.DefaultDaysPerYear,
                IsPaid = request.IsPaid,
                IsActive = true,
                Description = request.Description
            };

            _context.LeaveTypes.Add(leaveType);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Ok(leaveType.Id);
        }
        catch (Exception ex)
        {
            return Result<int>.Fail($"Error creating leave type: {ex.Message}");
        }
    }
}
