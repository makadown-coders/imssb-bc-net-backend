using Application.Features.Solicitudes;
using FluentAssertions;

namespace Application.Tests;

public sealed class CatalogoSiciliaTests
{
    private readonly OncoClaseUpsertRequestValidator _claseValidator = new();
    private readonly OncoSubclaseUpsertRequestValidator _subclaseValidator = new();

    [Fact]
    public void ClaseValidator_AcceptsValidRequest()
    {
        var request = new OncoClaseUpsertRequest("I", "Clase I", null, 2m, true);

        _claseValidator.Validate(request).IsValid.Should().BeTrue();
    }

    [Fact]
    public void ClaseValidator_RejectsInvalidCodeNameAndStockFactor()
    {
        var request = new OncoClaseUpsertRequest(
            "CODIGO_DEMASIADO_LARGO",
            string.Empty,
            null,
            -0.01m,
            true);

        var result = _claseValidator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Select(error => error.PropertyName)
            .Should()
            .Contain([nameof(request.Codigo), nameof(request.Nombre), nameof(request.StockFactor)]);
    }

    [Fact]
    public void SubclaseValidator_RejectsMissingCodeAndLongName()
    {
        var request = new OncoSubclaseUpsertRequest(null, new string('A', 151), null, true);

        var result = _subclaseValidator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Select(error => error.PropertyName)
            .Should()
            .Contain([nameof(request.Codigo), nameof(request.Nombre)]);
    }

    [Theory]
    [InlineData(null, null, 1, 50, 0)]
    [InlineData(2, 25, 2, 25, 25)]
    [InlineData(0, 500, 1, 200, 0)]
    public void Pagination_NormalizesValues(
        int? page,
        int? pageSize,
        int expectedPage,
        int expectedPageSize,
        int expectedOffset)
    {
        CatalogoSiciliaPagination.Normalize(page, pageSize)
            .Should()
            .Be((expectedPage, expectedPageSize, expectedOffset));
    }
}
