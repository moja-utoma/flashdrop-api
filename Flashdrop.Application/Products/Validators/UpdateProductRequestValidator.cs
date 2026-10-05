using Flashdrop.Application.Products.DTOs.Requests;
using FluentValidation;

namespace Flashdrop.Application.Products.Validators;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.BasePrice)
            .GreaterThan(0);

        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("Category must be a valid product category.");
    }
}
