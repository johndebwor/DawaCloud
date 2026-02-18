using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.HR.Commands;

public record CreateAttendanceCommand(
    int EmployeeId,
    DateTime Date,
    DateTime? ClockIn,
    DateTime? ClockOut,
    AttendanceStatus Status,
    string? Notes
) : IRequest<Result<int>>;

public class CreateAttendanceCommandHandler : IRequestHandler<CreateAttendanceCommand, Result<int>>
{
    private readonly AppDbContext _context;

    public CreateAttendanceCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateAttendanceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == request.EmployeeId && a.Date.Date == request.Date.Date, cancellationToken);

            if (existing != null)
            {
                existing.ClockIn = request.ClockIn ?? existing.ClockIn;
                existing.ClockOut = request.ClockOut ?? existing.ClockOut;
                existing.Status = request.Status;
                existing.Notes = request.Notes;

                await _context.SaveChangesAsync(cancellationToken);
                return Result<int>.Ok(existing.Id);
            }

            var attendance = new Attendance
            {
                EmployeeId = request.EmployeeId,
                Date = request.Date.Date,
                ClockIn = request.ClockIn,
                ClockOut = request.ClockOut,
                Status = request.Status,
                Notes = request.Notes
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Ok(attendance.Id);
        }
        catch (Exception ex)
        {
            return Result<int>.Fail($"Error recording attendance: {ex.Message}");
        }
    }
}
