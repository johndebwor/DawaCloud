using DawaCloud.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Wholesale.Commands;

public record DeleteCustomerCommand(int Id) : IRequest<Result>;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result>
{
    private readonly AppDbContext _context;

    public DeleteCustomerCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken ct)
    {
        var customer = await _context.WholesaleCustomers
            .FirstOrDefaultAsync(c => c.Id == request.Id, ct);

        if (customer == null)
            return Result.Fail("Customer not found");

        // Soft delete
        customer.IsDeleted = true;
        customer.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
