using FluentValidation;

namespace ProductService.Application.Features.Products.Commands;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("{PropertyName} alanı boş olamaz.")
            .NotNull()
            .MaximumLength(50).WithMessage("{PropertyName} en fazla 50 karakter olabilir.");

        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage("{PropertyName} 0'dan büyük olmalıdır.");

        RuleFor(p => p.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} negatif olamaz.");

        RuleFor(p => p.PictureUrl)
            .NotEmpty().WithMessage("{PropertyName} gereklidir.");
    }
}