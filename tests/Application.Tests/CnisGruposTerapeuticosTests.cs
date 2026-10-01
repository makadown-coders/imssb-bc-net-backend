using System.Data.Common;
using Application.Features.Solicitudes;
using FluentAssertions;
using Infrastructure.Persistence.Solicitudes;
using Infrastructure.Services.Solicitudes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace Application.Tests;

public sealed class CnisGruposTerapeuticosTests
{
    [Fact]
    public async Task Handlers_PropagateResultsFilterAndCancellationToken()
    {
        using var cancellation = new CancellationTokenSource();
        var token = cancellation.Token;
        var service = new Mock<ICnisGruposTerapeuticosService>(MockBehavior.Strict);
        IReadOnlyList<CnisGrupoTerapeuticoDto> grupos = [new(16, "Oncología")];
        IReadOnlyList<CnisArticuloGrupoDto> asociaciones = [new("010.000.0002.00", 16, "Oncología"), new("010.000.0002.00", 23, "Cuidados Paliativos")];
        IReadOnlyList<CnisArticuloDto> articulos = [new("010.000.0002.00", "Descripción operativa", null)];
        service.Setup(s => s.GetGruposAsync(token)).ReturnsAsync(grupos);
        service.Setup(s => s.GetAsociacionesAsync(token)).ReturnsAsync(asociaciones);
        service.Setup(s => s.GetArticulosAsync(16, token)).ReturnsAsync(articulos);

        (await new GetCnisGruposTerapeuticosQueryHandler(service.Object)
            .Handle(new(), token)).Should().BeSameAs(grupos);
        (await new GetCnisArticuloGruposQueryHandler(service.Object)
            .Handle(new(), token)).Should().BeSameAs(asociaciones);
        (await new GetCnisArticulosPorGrupoQueryHandler(service.Object)
            .Handle(new(16), token)).Should().BeSameAs(articulos);
        service.VerifyAll();
    }

    [PostgresFact]
    public async Task Queries_JoinNormalizeFilterAndDeduplicateInPostgres()
    {
        var interceptor = new FixtureInterceptor();
        await using var db = CreateContext(interceptor);
        var service = new CnisGruposTerapeuticosService(db);

        var grupos = await service.GetGruposAsync(CancellationToken.None);
        grupos.Should().Equal(new CnisGrupoTerapeuticoDto(1, "Analgesia"),
            new(16, "Oncología"), new(23, "Cuidados Paliativos"));

        var asociaciones = await service.GetAsociacionesAsync(CancellationToken.None);
        asociaciones.Should().Equal(new CnisArticuloGrupoDto("010.000.0001.00", 1, "Analgesia"),
            new(" 010-000-0002-00 ", 16, "Oncología"),
            new(" 010-000-0002-00 ", 23, "Cuidados Paliativos"));

        var articulos = await service.GetArticulosAsync(16, CancellationToken.None);
        articulos.Should().Equal(new CnisArticuloDto(" 010-000-0002-00 ", "Descripción operativa", "Caja"));
        (await service.GetArticulosAsync(999, CancellationToken.None)).Should().BeEmpty();
        (await service.GetArticulosAsync(1, CancellationToken.None)).Should()
            .Equal(new CnisArticuloDto("010.000.0001.00", null, null));
        interceptor.Commands.Should().Be(5);
        db.ChangeTracker.Entries().Should().BeEmpty();
    }

    [PostgresFact]
    public async Task Queries_RespectCancellation()
    {
        await using var db = CreateContext(new FixtureInterceptor());
        var service = new CnisGruposTerapeuticosService(db);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => service.GetGruposAsync(cancellation.Token));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => service.GetAsociacionesAsync(cancellation.Token));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => service.GetArticulosAsync(16, cancellation.Token));
    }

    private static SolicitudesDbContext CreateContext(FixtureInterceptor interceptor) =>
        new(new DbContextOptionsBuilder<SolicitudesDbContext>()
            .UseNpgsql(Environment.GetEnvironmentVariable("CNIS_TEST_CONNECTION_STRING"))
            .AddInterceptors(interceptor).Options);

    // Sustituye únicamente las fuentes por CTEs de datos ficticios. PostgreSQL ejecuta
    // los JOIN, regexp_replace, DISTINCT, parámetros y orden del servicio sin cambios.
    private sealed class FixtureInterceptor : DbCommandInterceptor
    {
        public int Commands { get; private set; }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            if (command.CommandText.Contains("WHERE g.numero"))
            {
                command.Parameters.Count.Should().Be(1);
                command.CommandText.Should().Contain("@p0");
            }

            command.CommandText = """
                WITH articulos_fixture(clave, clavea, descripcion, presentacion) AS (VALUES
                    ('010.000.0001.00', 'ignorar', NULL::text, NULL::text),
                    (' 010-000-0002-00 ', 'ignorar', 'Descripción operativa', 'Caja'),
                    (' 010-000-0002-00 ', 'ignorar', 'Descripción operativa', 'Caja'),
                    ('999', '010.000.0001.00', 'No existe en CNIS', 'Caja'),
                    ('020.000.0003.00', 'ignorar', 'Material de curación', 'Pieza'),
                    (NULL, '010.000.0001.00', 'Sin clave', 'Caja')),
                insumo_fixture(id, clave_normalizada) AS (VALUES
                    (1, '010000000100'), (2, '010000000200'), (3, '020000000300'), (4, 'ausente')),
                insumo_grupo_fixture(insumo_id, grupo_id) AS (VALUES
                    (1, 10), (2, 20), (2, 30), (2, 20), (4, 40)),
                grupo_fixture(id, numero, nombre) AS (VALUES
                    (10, 1, 'Analgesia'), (20, 16, 'Oncología'), (30, 23, 'Cuidados Paliativos'),
                    (40, 2, 'Sin artículos'))
                """ + "\n" + command.CommandText
                    .Replace("public.articulos", "articulos_fixture")
                    .Replace("cnis.insumo_grupo", "insumo_grupo_fixture")
                    .Replace("cnis.insumo", "insumo_fixture")
                    .Replace("cnis.grupo", "grupo_fixture");
            Commands++;
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }
    }
}

public sealed class PostgresFactAttribute : FactAttribute
{
    public PostgresFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CNIS_TEST_CONNECTION_STRING")))
            Skip = "Defina CNIS_TEST_CONNECTION_STRING para ejecutar las consultas en PostgreSQL con CTEs de prueba (sin crear tablas).";
    }
}
