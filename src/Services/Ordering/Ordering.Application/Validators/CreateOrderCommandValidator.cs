using FluentValidation;
using Ordering.Application.Commands;

namespace Ordering.Application.Validators;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId es requerido");

        RuleFor(x => x.UserEmail)
            .NotEmpty().WithMessage("Email es requerido")
            .EmailAddress().WithMessage("Email inválido");

        RuleFor(x => x.ShippingAddress)
            .NotNull().WithMessage("Dirección de envío es requerida");

        RuleFor(x => x.ShippingAddress.Street)
            .NotEmpty().WithMessage("Calle es requerida")
            .MaximumLength(200);

        RuleFor(x => x.ShippingAddress.City)
            .NotEmpty().WithMessage("Ciudad es requerida")
            .MaximumLength(100);

        RuleFor(x => x.ShippingAddress.Country)
            .NotEmpty().WithMessage("País es requerido")
            .MaximumLength(100);

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Debe tener al menos un item")
            .Must(items => items.Count > 0).WithMessage("Debe tener al menos un item");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId es requerido");

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Cantidad debe ser mayor a 0");

            item.RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Precio debe ser mayor a 0");
        });
    }
}