using FluentValidation;

namespace Application.Features.Solicitudes;

public sealed class CrearBitacoraRequestValidator : AbstractValidator<CrearBitacoraRequest>
{
    public CrearBitacoraRequestValidator()
    {
        RuleFor(request => request.Cluesimb).NotEmpty().MaximumLength(20);
        RuleFor(request => request.TipoPedido).Must(value => value is "Ordinario" or "Extraordinario")
            .WithMessage("El tipo de pedido debe ser Ordinario o Extraordinario.");
        RuleFor(request => request.TipoInsumo).NotEmpty().MaximumLength(250);
        RuleFor(request => request.Periodo).MaximumLength(150);
        RuleFor(request => request.Articulos).NotEmpty().Must(items => items is { Count: <= 10000 })
            .WithMessage("La solicitud no puede contener más de 10,000 artículos.");
        RuleForEach(request => request.Articulos).ChildRules(article =>
        {
            article.RuleFor(item => item.Clave).NotEmpty().MaximumLength(50);
            article.RuleFor(item => item.Cantidad).GreaterThan(0);
        });
    }
}
