using Basket.Application.DTOs;
using FluentValidation;

namespace Basket.Application.Validators;

public class AddItemToBasketValidator : AbstractValidator<AddItemToBasketDto>
{
    public AddItemToBasketValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("El ProductId es requerido");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("El nombre del producto es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0")
            .LessThanOrEqualTo(100).WithMessage("La cantidad no puede exceder 100 unidades");
    }
}

public class UpdateItemQuantityValidator : AbstractValidator<UpdateItemQuantityDto>
{
    public UpdateItemQuantityValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("El ProductId es requerido");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0")
            .LessThanOrEqualTo(100).WithMessage("La cantidad no puede exceder 100 unidades");
    }
}