using Flashdrop.Application.Sales.DTOs.Requests;
using FluentValidation;

namespace Flashdrop.Application.Sales.Validators;

public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    public UpdateSaleRequestValidator()
    {
        RuleFor(x => x.TotalStock)
            .GreaterThan(0)
            .WithMessage("Total stock must be greater than zero");
        
        RuleFor(x => x.PricePerUnit)
            .GreaterThan(0)
            .WithMessage("Price per unit must be greater than zero");
        
        RuleFor(x => x.MaxUnitsPerUser)
            .GreaterThan(0)
            .WithMessage("Max unit per user must be greater than zero");
        
        RuleFor(x => x.StartsAt)
            .GreaterThan(DateTimeOffset.UtcNow)
            .WithMessage("Start time must be later than now");

        RuleFor(x => x.EndsAt)
            .GreaterThan(DateTimeOffset.UtcNow)
            .WithMessage("End time must be later than now");

        RuleFor(x => x)
            .Must(x =>
                x.EndsAt < DateTimeOffset.UtcNow
                || x.StartsAt < DateTimeOffset.UtcNow
                || x.EndsAt > x.StartsAt)
            .WithMessage("Start time must be later than end time");
        
        RuleFor(x => x.RowVersion)
            .NotNull()
            .NotEmpty()
            .WithMessage("Row version is required");
    }
}