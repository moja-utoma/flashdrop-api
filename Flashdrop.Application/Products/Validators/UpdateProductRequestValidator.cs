using Flashdrop.Application.Products.DTOs.Requests;
using FluentValidation;

namespace Flashdrop.Application.Products.Validators;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Product name is required.")
            .MaximumLength(200)
                .WithMessage("Product name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
                .WithMessage("Product description is required.");

        RuleFor(x => x.BasePrice)
            .GreaterThan(0)
                .WithMessage("Base price must be greater than zero.");

        RuleFor(x => x.Category)
            .IsInEnum()
                .WithMessage("Category must be a valid product category.");
    }
}
