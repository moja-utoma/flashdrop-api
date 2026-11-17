using Flashdrop.Application.Sales.DTOs.Requests;
using FluentValidation;

namespace Flashdrop.Application.Sales.Validators;

public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product Id is required");

        RuleFor(x => x.TotalStock)
            .GreaterThan(0)
            .WithMessage("Total Stock must be greater than 0");

        RuleFor(x => x.PricePerUnit)
            .GreaterThan(0)
            .WithMessage("Price per unit must be greater than 0");

        RuleFor(x => x.MaxUnitsPerUser)
            .GreaterThan(0)
            .WithMessage("Max Unit per User must be greater than 0");

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
    }
}