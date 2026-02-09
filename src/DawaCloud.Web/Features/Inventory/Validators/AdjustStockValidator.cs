using DawaCloud.Web.Features.Inventory.Commands;
using FluentValidation;

namespace DawaCloud.Web.Features.Inventory.Validators;

public class AdjustStockValidator : AbstractValidator<AdjustStockCommand>
{
    public AdjustStockValidator()
    {
        RuleFor(x => x.BatchId)
            .GreaterThan(0)
            .WithMessage("Please select a valid batch");

        RuleFor(x => x.QuantityChange)
            .NotEqual(0)
            .WithMessage("Quantity change cannot be zero");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Please provide a reason for the adjustment")
            .MaximumLength(200)
            .WithMessage("Reason cannot exceed 200 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes))
            .WithMessage("Notes cannot exceed 500 characters");
    }
}
