using System.Text.Json.Serialization;

namespace Application.Features.Solicitudes;

public sealed record HomologoCrudRowDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("clave")] string Clave,
    [property: JsonPropertyName("sustituto")] string Sustituto,
    [property: JsonPropertyName("factor")] string Factor);

public sealed record HomologoCrudListResponse(
    [property: JsonPropertyName("rows")] IReadOnlyList<HomologoCrudRowDto> Rows);

public sealed record HomologoCrudSingleResponse(
    [property: JsonPropertyName("row")] HomologoCrudRowDto Row);

public sealed record HomologoCrudDeleteResponse(
    [property: JsonPropertyName("ok")] bool Ok);

public sealed record HomologoCrudUpsertRequest(
    [property: JsonPropertyName("clave")] string? Clave,
    [property: JsonPropertyName("sustituto")] string? Sustituto,
    [property: JsonPropertyName("factor")] string? Factor);

public interface IHomologosCrudService
{
    Task<HomologoCrudListResponse> ListAsync(CancellationToken cancellationToken);
    Task<HomologoCrudSingleResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<HomologoCrudSingleResponse> CreateAsync(HomologoCrudUpsertRequest request, CancellationToken cancellationToken);
    Task<HomologoCrudSingleResponse?> UpdateAsync(int id, HomologoCrudUpsertRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
