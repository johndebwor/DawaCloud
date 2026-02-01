using DawaFlow.Web.Data;
using DawaFlow.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Drugs.Commands;

public record CreateDrugCommand(
    string Code,
    string Name,
    string? Description,
    int CategoryId,
    string? Manufacturer,
    decimal CostPrice,
    decimal WholesalePrice,
    decimal RetailPrice,
    decimal TaxRate,
    int ReorderLevel,
    string? DosageForm,
    bool RequiresPrescription
) : IRequest<CreateDrugResult>;

public record CreateDrugResult(bool Success, string Message, int? DrugId = null);

public class CreateDrugCommandHandler : IRequestHandler<CreateDrugCommand, CreateDrugResult>
{
    private readonly AppDbContext _context;
    private readonly ILogger<CreateDrugCommandHandler> _logger;

    public CreateDrugCommandHandler(AppDbContext context, ILogger<CreateDrugCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CreateDrugResult> Handle(CreateDrugCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if code already exists
            var existingDrug = await _context.Drugs
                .FirstOrDefaultAsync(d => d.Code == request.Code, cancellationToken);

            if (existingDrug != null)
            {
                return new CreateDrugResult(false, $"A drug with code '{request.Code}' already exists.");
            }

            var drug = new Drug
            {
                Code = request.Code,
                Name = request.Name,
                GenericName = request.Name, // Default to name
                Description = request.Description,
                CategoryId = request.CategoryId,
                Manufacturer = request.Manufacturer,
                CostPrice = request.CostPrice,
                WholesalePrice = request.WholesalePrice,
                RetailPrice = request.RetailPrice,
                TaxRate = request.TaxRate,
                ReorderLevel = request.ReorderLevel,
                DosageForm = request.DosageForm,
                RequiresPrescription = request.RequiresPrescription,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Drugs.Add(drug);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Drug created: {Code} - {Name} (ID: {Id})", drug.Code, drug.Name, drug.Id);

            return new CreateDrugResult(true, "Drug created successfully.", drug.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating drug: {Code}", request.Code);
            return new CreateDrugResult(false, $"Error creating drug: {ex.Message}");
        }
    }
}

public record UpdateDrugCommand(
    int Id,
    string Code,
    string Name,
    string? Description,
    int CategoryId,
    string? Manufacturer,
    decimal CostPrice,
    decimal WholesalePrice,
    decimal RetailPrice,
    decimal TaxRate,
    int ReorderLevel,
    string? DosageForm,
    bool RequiresPrescription
) : IRequest<UpdateDrugResult>;

public record UpdateDrugResult(bool Success, string Message);

public class UpdateDrugCommandHandler : IRequestHandler<UpdateDrugCommand, UpdateDrugResult>
{
    private readonly AppDbContext _context;
    private readonly ILogger<UpdateDrugCommandHandler> _logger;

    public UpdateDrugCommandHandler(AppDbContext context, ILogger<UpdateDrugCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UpdateDrugResult> Handle(UpdateDrugCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var drug = await _context.Drugs.FindAsync(new object[] { request.Id }, cancellationToken);

            if (drug == null)
            {
                return new UpdateDrugResult(false, "Drug not found.");
            }

            // Check if code is being changed and if it conflicts
            if (drug.Code != request.Code)
            {
                var existingDrug = await _context.Drugs
                    .FirstOrDefaultAsync(d => d.Code == request.Code && d.Id != request.Id, cancellationToken);

                if (existingDrug != null)
                {
                    return new UpdateDrugResult(false, $"A drug with code '{request.Code}' already exists.");
                }
            }

            drug.Code = request.Code;
            drug.Name = request.Name;
            drug.Description = request.Description;
            drug.CategoryId = request.CategoryId;
            drug.Manufacturer = request.Manufacturer;
            drug.CostPrice = request.CostPrice;
            drug.WholesalePrice = request.WholesalePrice;
            drug.RetailPrice = request.RetailPrice;
            drug.TaxRate = request.TaxRate;
            drug.ReorderLevel = request.ReorderLevel;
            drug.DosageForm = request.DosageForm;
            drug.RequiresPrescription = request.RequiresPrescription;
            drug.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Drug updated: {Code} - {Name} (ID: {Id})", drug.Code, drug.Name, drug.Id);

            return new UpdateDrugResult(true, "Drug updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating drug: {Id}", request.Id);
            return new UpdateDrugResult(false, $"Error updating drug: {ex.Message}");
        }
    }
}
