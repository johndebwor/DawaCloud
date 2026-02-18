using DawaCloud.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.HR.Queries;

public record GetLeaveTypesQuery : IRequest<List<LeaveTypeDto>>;

public class LeaveTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DefaultDaysPerYear { get; set; }
    public bool IsPaid { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}

public class GetLeaveTypesQueryHandler : IRequestHandler<GetLeaveTypesQuery, List<LeaveTypeDto>>
{
    private readonly AppDbContext _context;

    public GetLeaveTypesQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LeaveTypeDto>> Handle(GetLeaveTypesQuery request, CancellationToken cancellationToken)
    {
        return await _context.LeaveTypes
            .AsNoTracking()
            .OrderBy(lt => lt.Name)
            .Select(lt => new LeaveTypeDto
            {
                Id = lt.Id,
                Name = lt.Name,
                DefaultDaysPerYear = lt.DefaultDaysPerYear,
                IsPaid = lt.IsPaid,
                IsActive = lt.IsActive,
                Description = lt.Description
            })
            .ToListAsync(cancellationToken);
    }
}
