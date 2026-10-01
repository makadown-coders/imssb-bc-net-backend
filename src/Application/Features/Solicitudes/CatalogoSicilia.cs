using FluentValidation;

namespace Application.Features.Solicitudes;

public sealed record OncoClaseDto(
    short Id,
    string Codigo,
    string Nombre,
    string? Descripcion,
    decimal? StockFactor,
    bool Activo,
    DateTime CreadoEn,
    DateTime ActualizadoEn);

public sealed record OncoSubclaseDto(
    short Id,
    string Codigo,
    string Nombre,
    string? Descripcion,
    bool Activo,
    DateTime CreadoEn,
    DateTime ActualizadoEn);

public sealed record OncoClaseUpsertRequest(
    string? Codigo,
    string? Nombre,
    string? Descripcion,
    decimal? StockFactor,
    bool Activo = true);

public sealed record OncoSubclaseUpsertRequest(
    string? Codigo,
    string? Nombre,
    string? Descripcion,
    bool Activo = true);

public sealed record CatalogoSiciliaPaginatedResponse<T>(
    int Count,
    int Total,
    int Page,
    int PageSize,
    int TotalPages,
    IReadOnlyList<T> Rows);

public sealed class OncoClaseUpsertRequestValidator : AbstractValidator<OncoClaseUpsertRequest>
{
    public OncoClaseUpsertRequestValidator()
    {
        RuleFor(request => request.Codigo)
            .NotEmpty()
            .MaximumLength(10);
        RuleFor(request => request.Nombre)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(request => request.StockFactor)
            .GreaterThanOrEqualTo(0)
            .When(request => request.StockFactor.HasValue);
    }
}

public sealed class OncoSubclaseUpsertRequestValidator : AbstractValidator<OncoSubclaseUpsertRequest>
{
    public OncoSubclaseUpsertRequestValidator()
    {
        RuleFor(request => request.Codigo)
            .NotEmpty()
            .MaximumLength(20);
        RuleFor(request => request.Nombre)
            .NotEmpty()
            .MaximumLength(150);
    }
}

public sealed class DuplicateOncoCatalogCodeException(string codigo)
    : Exception($"Ya existe un registro con el código '{codigo}'.");

public interface ICatalogoSiciliaService
{
    Task<CatalogoSiciliaPaginatedResponse<OncoClaseDto>> ListClasesAsync(
        string? q,
        bool? activo,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken);

    Task<OncoClaseDto?> GetClaseAsync(int id, CancellationToken cancellationToken);
    Task<OncoClaseDto> CreateClaseAsync(OncoClaseUpsertRequest request, CancellationToken cancellationToken);
    Task<OncoClaseDto?> UpdateClaseAsync(int id, OncoClaseUpsertRequest request, CancellationToken cancellationToken);
    Task<bool> DeactivateClaseAsync(int id, CancellationToken cancellationToken);

    Task<CatalogoSiciliaPaginatedResponse<OncoSubclaseDto>> ListSubclasesAsync(
        string? q,
        bool? activo,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken);

    Task<OncoSubclaseDto?> GetSubclaseAsync(int id, CancellationToken cancellationToken);
    Task<OncoSubclaseDto> CreateSubclaseAsync(OncoSubclaseUpsertRequest request, CancellationToken cancellationToken);
    Task<OncoSubclaseDto?> UpdateSubclaseAsync(int id, OncoSubclaseUpsertRequest request, CancellationToken cancellationToken);
    Task<bool> DeactivateSubclaseAsync(int id, CancellationToken cancellationToken);
}

public static class CatalogoSiciliaPagination
{
    public static (int Page, int PageSize, int Offset) Normalize(int? page, int? pageSize)
    {
        var normalizedPage = Math.Max(page ?? 1, 1);
        var normalizedPageSize = Math.Clamp(pageSize ?? 50, 1, 200);
        return (normalizedPage, normalizedPageSize, (normalizedPage - 1) * normalizedPageSize);
    }
}
