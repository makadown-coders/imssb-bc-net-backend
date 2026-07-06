using Application.Features.Solicitudes;
using FluentAssertions;

namespace Application.Tests;

public sealed class CrearBitacoraRequestValidatorTests
{
    private readonly CrearBitacoraRequestValidator validator = new();

    [Fact]
    public async Task ValidRequest_PassesValidation()
    {
        var request = new CrearBitacoraRequest(
            "BCIMB000001",
            "Ordinario",
            "Medicamento",
            "2026-07-01 - 2026-07-31",
            [new ArticuloBitacoraRequest("010.000.0001", 5)]);

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task EmptyArticles_AndInvalidOrderType_FailValidation()
    {
        var request = new CrearBitacoraRequest("BCIMB000001", "Urgente", "Medicamento", null, []);

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(request.TipoPedido));
        result.Errors.Should().Contain(error => error.PropertyName == nameof(request.Articulos));
    }

    [Fact]
    public async Task ArticleWithZeroQuantity_FailsValidation()
    {
        var request = new CrearBitacoraRequest(
            "BCIMB000001", "Ordinario", "Medicamento", null,
            [new ArticuloBitacoraRequest("010.000.0001", 0)]);

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName.EndsWith("Cantidad"));
    }
}
