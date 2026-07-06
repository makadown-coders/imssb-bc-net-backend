using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Application.Features.Solicitudes;
using Domain.Entities.Solicitudes;
using Infrastructure.Persistence.Solicitudes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure.Services.Solicitudes;

internal sealed class SolicitudesCaptureService(
    SolicitudesDbContext dbContext,
    IConfiguration configuration) : ISolicitudesCaptureService
{
    public async Task<IReadOnlyList<UnidadSolicitudDto>> GetUnidadesAsync(CancellationToken cancellationToken) =>
        await dbContext.VUnidadMedicaDetalles.AsNoTracking()
            .OrderBy(item => item.NombreMunicipio)
            .ThenBy(item => item.NombreLocalidad)
            .ThenBy(item => item.NombreDeUnidad)
            .Select(item => new UnidadSolicitudDto(
                item.Id ?? 0,
                item.Cluessa,
                item.Cluesimb,
                item.NombreMunicipio,
                item.NombreLocalidad,
                item.NombreTipologia,
                item.EsSegundoNivel,
                item.NombreDeUnidad,
                item.TipoUnidad,
                item.AliasSas,
                item.Direccion,
                item.Latitud,
                item.Longitud,
                item.EstratoUnidad,
                item.NivelAtencion))
            .ToListAsync(cancellationToken);

    public async Task<BuscarArticulosResponse> BuscarArticulosAsync(string query, CancellationToken cancellationToken)
    {
        var normalized = query.Trim();
        var filtered = dbContext.Articulos.AsNoTracking().Where(item =>
            EF.Functions.ILike(item.Clave ?? string.Empty, $"%{normalized}%") ||
            EF.Functions.ILike(item.Descripcion ?? string.Empty, $"%{normalized}%"));

        var total = await filtered.CountAsync(cancellationToken);
        var results = await filtered
            .OrderBy(item => item.Clave == null)
            .ThenBy(item => item.Clave)
            .Take(12)
            .Select(item => new ArticuloSolicitudDto(
                item.Clave ?? string.Empty,
                item.Descripcion ?? string.Empty,
                item.Presentacion ?? string.Empty))
            .ToListAsync(cancellationToken);

        return new BuscarArticulosResponse(results, total);
    }

    public async Task<CrearBitacoraResponse> CrearBitacoraAsync(
        CrearBitacoraRequest request,
        CancellationToken cancellationToken)
    {
        var canonical = Canonicalize(request);
        var hash = ComputeHash(canonical);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var existingId = await dbContext.SolicitudBitacoras.AsNoTracking()
            .Where(item => item.Cluesimb == canonical.Cluesimb && item.CreatedDay == today && item.PayloadHash == hash)
            .Select(item => (Guid?)item.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (existingId.HasValue)
        {
            return new CrearBitacoraResponse(true, existingId.Value, true, hash);
        }

        var header = new SolicitudBitacora
        {
            Cluesimb = canonical.Cluesimb,
            TipoPedido = canonical.TipoPedido,
            TiposInsumo = canonical.TiposInsumo,
            PeriodoTexto = string.IsNullOrWhiteSpace(canonical.Periodo) ? null : canonical.Periodo,
            ExportKind = "raw",
            TotalRenglones = canonical.Articulos.Count,
            TotalPiezas = canonical.Articulos.Sum(item => item.Cantidad),
            PayloadHash = hash
        };
        header.SolicitudBitacoraDetalles = canonical.Articulos.Select(item => new SolicitudBitacoraDetalle
        {
            Clave = item.Clave,
            Cantidad = item.Cantidad,
            UnidadMedida = null
        }).ToList();

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        dbContext.SolicitudBitacoras.Add(header);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return new CrearBitacoraResponse(true, header.Id, false, hash);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            await transaction.RollbackAsync(cancellationToken);
            dbContext.ChangeTracker.Clear();
            var concurrentId = await dbContext.SolicitudBitacoras.AsNoTracking()
                .Where(item => item.Cluesimb == canonical.Cluesimb && item.CreatedDay == today && item.PayloadHash == hash)
                .Select(item => item.Id)
                .SingleAsync(cancellationToken);
            return new CrearBitacoraResponse(true, concurrentId, true, hash);
        }
    }

    private CanonicalRequest Canonicalize(CrearBitacoraRequest request)
    {
        var articles = request.Articulos
            .Select(item => new CanonicalArticle(item.Clave.Trim().ToUpperInvariant(), item.Cantidad))
            .OrderBy(item => item.Clave, StringComparer.Ordinal)
            .ToList();
        var types = request.TipoInsumo.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(CollapseWhitespace).Distinct(StringComparer.Ordinal).OrderBy(item => item, StringComparer.Ordinal).ToList();
        return new CanonicalRequest(
            request.Cluesimb.Trim().ToUpperInvariant(),
            CollapseWhitespace(request.TipoPedido),
            types,
            CollapseWhitespace(request.Periodo ?? string.Empty),
            articles);
    }

    private string ComputeHash(CanonicalRequest request)
    {
        var salt = Environment.GetEnvironmentVariable("SOLICITUDES_HASH_SALT")
            ?? configuration["Solicitudes:HashSalt"];
        if (string.IsNullOrWhiteSpace(salt))
        {
            throw new InvalidOperationException("SOLICITUDES_HASH_SALT no está configurado.");
        }
        var json = JsonSerializer.Serialize(new
        {
            cluesimb = request.Cluesimb,
            tipoPedido = request.TipoPedido,
            tiposInsumo = request.TiposInsumo,
            periodo = request.Periodo,
            articulos = request.Articulos.Select(item => new { clave = item.Clave, cantidad = item.Cantidad })
        });
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{json}|{salt}"))).ToLowerInvariant();
    }

    private static string CollapseWhitespace(string value) => string.Join(' ', value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    private sealed record CanonicalArticle(string Clave, decimal Cantidad);
    private sealed record CanonicalRequest(string Cluesimb, string TipoPedido, List<string> TiposInsumo, string Periodo, List<CanonicalArticle> Articulos);
}
