using System;
using System.Collections.Generic;
using Domain.Entities.Solicitudes;
using Microsoft.EntityFrameworkCore;
using MonitorEntity = Domain.Entities.Solicitudes.Monitor;

namespace Infrastructure.Persistence.Solicitudes;

public partial class SolicitudesDbContext : DbContext
{
    public SolicitudesDbContext(DbContextOptions<SolicitudesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Articulo> Articulos { get; set; }

    public virtual DbSet<AsignacionDispositivo> AsignacionDispositivos { get; set; }

    public virtual DbSet<BalanceoApartadosHistorial> BalanceoApartadosHistorials { get; set; }

    public virtual DbSet<BalanceoDetalladoFinal> BalanceoDetalladoFinals { get; set; }

    public virtual DbSet<CatPerifericoTipo> CatPerifericoTipos { get; set; }

    public virtual DbSet<Cita> Citas { get; set; }

    public virtual DbSet<Cpm> Cpms { get; set; }

    public virtual DbSet<CpmReal> CpmReals { get; set; }

    public virtual DbSet<Dispositivo> Dispositivos { get; set; }

    public virtual DbSet<Entradum> Entrada { get; set; }

    public virtual DbSet<EstadoDispositivo> EstadoDispositivos { get; set; }

    public virtual DbSet<FactoresConversion> FactoresConversions { get; set; }

    public virtual DbSet<FeatureFlag> FeatureFlags { get; set; }

    public virtual DbSet<Homologo> Homologos { get; set; }

    public virtual DbSet<InventarioInicial> InventarioInicials { get; set; }

    public virtual DbSet<Kit> Kits { get; set; }

    public virtual DbSet<KitClave> KitClaves { get; set; }

    public virtual DbSet<Localidad> Localidads { get; set; }

    public virtual DbSet<LogEjecucionesBalanceo> LogEjecucionesBalanceos { get; set; }

    public virtual DbSet<MonitorEntity> Monitors { get; set; }

    public virtual DbSet<Municipio> Municipios { get; set; }

    public virtual DbSet<OncoClafe> OncoClaves { get; set; }

    public virtual DbSet<OncoClavesBase> OncoClavesBases { get; set; }

    public virtual DbSet<OncoClase> OncoClases { get; set; }

    public virtual DbSet<OncoSubclase> OncoSubclases { get; set; }

    public virtual DbSet<OncoUnidade> OncoUnidades { get; set; }

    public virtual DbSet<Periferico> Perifericos { get; set; }

    public virtual DbSet<PermBalanceoDetalladoFinal> PermBalanceoDetalladoFinals { get; set; }

    public virtual DbSet<PermBalanceoResultado> PermBalanceoResultados { get; set; }

    public virtual DbSet<PermResumenAlmacenesFinal> PermResumenAlmacenesFinals { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<PersonaCorreo> PersonaCorreos { get; set; }

    public virtual DbSet<RadarEvento> RadarEventos { get; set; }

    public virtual DbSet<RadarEventoClafe> RadarEventoClaves { get; set; }

    public virtual DbSet<ResumenAlmacenesFinal> ResumenAlmacenesFinals { get; set; }

    public virtual DbSet<Salidum> Salida { get; set; }

    public virtual DbSet<SolicitudBitacora> SolicitudBitacoras { get; set; }

    public virtual DbSet<SolicitudBitacoraDetalle> SolicitudBitacoraDetalles { get; set; }

    public virtual DbSet<TipoDispositivo> TipoDispositivos { get; set; }

    public virtual DbSet<TipoUnidad> TipoUnidads { get; set; }

    public virtual DbSet<TmpExistencia> TmpExistencias { get; set; }

    public virtual DbSet<Traspaso> Traspasos { get; set; }

    public virtual DbSet<UnidadMedica> UnidadMedicas { get; set; }

    public virtual DbSet<UnidadMedicaAlias> UnidadMedicaAliases { get; set; }

    public virtual DbSet<UnidadMedicaKit> UnidadMedicaKits { get; set; }

    public virtual DbSet<VCpmBc> VCpmBcs { get; set; }

    public virtual DbSet<VCpmDiferencia> VCpmDiferencias { get; set; }

    public virtual DbSet<VCpmReal> VCpmReals { get; set; }

    public virtual DbSet<VExistenciasConsolidada> VExistenciasConsolidadas { get; set; }

    public virtual DbSet<VMovimientosAUnidadesDesdeAbasto> VMovimientosAUnidadesDesdeAbastos { get; set; }

    public virtual DbSet<VOncoAbastoCpm> VOncoAbastoCpms { get; set; }

    public virtual DbSet<VReporteBalanceoJurisdiccional> VReporteBalanceoJurisdiccionals { get; set; }

    public virtual DbSet<VUnidadCpm> VUnidadCpms { get; set; }

    public virtual DbSet<VUnidadKitClavesExpectedVsCpm> VUnidadKitClavesExpectedVsCpms { get; set; }

    public virtual DbSet<VUnidadKitClavesExpectedVsCpmV2> VUnidadKitClavesExpectedVsCpmV2s { get; set; }

    public virtual DbSet<VUnidadMedicaDetalle> VUnidadMedicaDetalles { get; set; }

    public virtual DbSet<VUnidadMedicaKitClafe> VUnidadMedicaKitClaves { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("app_role_scope_type", new[] { "GLOBAL", "CLUES", "DEPTO" })
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum("auth", "oauth_authorization_status", new[] { "pending", "approved", "denied", "expired" })
            .HasPostgresEnum("auth", "oauth_client_type", new[] { "public", "confidential" })
            .HasPostgresEnum("auth", "oauth_registration_type", new[] { "dynamic", "manual" })
            .HasPostgresEnum("auth", "oauth_response_type", new[] { "code" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("flag_scope", new[] { "global", "nivel", "clues" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in", "like", "ilike", "is", "match", "imatch", "isdistinct" })
            .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS", "VECTOR" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("pg_catalog", "pg_cron")
            .HasPostgresExtension("pg_trgm")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<Articulo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("articulos_pkey");

            entity.ToTable("articulos");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasMaxLength(100)
                .HasColumnName("activo");
            entity.Property(e => e.Articulo1).HasColumnName("articulo");
            entity.Property(e => e.Categoria)
                .HasMaxLength(100)
                .HasColumnName("categoria");
            entity.Property(e => e.Cbf)
                .HasMaxLength(100)
                .HasColumnName("cbf");
            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .HasColumnName("clave");
            entity.Property(e => e.Clavea)
                .HasMaxLength(50)
                .HasColumnName("clavea");
            entity.Property(e => e.Codigobarras)
                .HasMaxLength(100)
                .HasColumnName("codigobarras");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Grupogasto).HasColumnName("grupogasto");
            entity.Property(e => e.Nivelatencion)
                .HasMaxLength(100)
                .HasColumnName("nivelatencion");
            entity.Property(e => e.Partida).HasColumnName("partida");
            entity.Property(e => e.Presentacion).HasColumnName("presentacion");
            entity.Property(e => e.Subgrupogasto).HasColumnName("subgrupogasto");
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(100)
                .HasColumnName("ubicacion");
        });

        modelBuilder.Entity<AsignacionDispositivo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("asignacion_dispositivo_pkey");

            entity.ToTable("asignacion_dispositivo");

            entity.HasIndex(e => new { e.DispositivoId, e.FechaAsignacion, e.Id }, "idx_asig_disp_dispositivo_fecha_id_desc").IsDescending(false, true, true);

            entity.HasIndex(e => e.LugarEspecifico, "idx_asig_lugar_trgm")
                .HasMethod("gin")
                .HasOperators(new[] { "gin_trgm_ops" });

            entity.HasIndex(e => e.DispositivoId, "idx_asignacion_activa").HasFilter("(fecha_retiro IS NULL)");

            entity.HasIndex(e => e.DispositivoId, "idx_asignacion_dispositivo_id");

            entity.HasIndex(e => e.EstadoDispositivoId, "idx_asignacion_estado_id");

            entity.HasIndex(e => e.EstadoDispositivoId, "idx_asignacion_estado_vigente").HasFilter("(fecha_retiro IS NULL)");

            entity.HasIndex(e => e.DispositivoId, "idx_asignacion_fecha_activa").HasFilter("(fecha_retiro IS NULL)");

            entity.HasIndex(e => e.PersonaId, "idx_asignacion_persona_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(100)
                .HasColumnName("creado_por");
            entity.Property(e => e.DispositivoId).HasColumnName("dispositivo_id");
            entity.Property(e => e.EstadoDispositivoId).HasColumnName("estado_dispositivo_id");
            entity.Property(e => e.FechaAsignacion)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_asignacion");
            entity.Property(e => e.FechaRetiro)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_retiro");
            entity.Property(e => e.LugarEspecifico)
                .HasMaxLength(100)
                .HasColumnName("lugar_especifico");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.PersonaId).HasColumnName("persona_id");

            entity.HasOne(d => d.Dispositivo).WithMany(p => p.AsignacionDispositivos)
                .HasForeignKey(d => d.DispositivoId)
                .HasConstraintName("asignacion_dispositivo_dispositivo_id_fkey");

            entity.HasOne(d => d.EstadoDispositivo).WithMany(p => p.AsignacionDispositivos)
                .HasForeignKey(d => d.EstadoDispositivoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("asignacion_dispositivo_estado_dispositivo_id_fkey");

            entity.HasOne(d => d.Persona).WithMany(p => p.AsignacionDispositivos)
                .HasForeignKey(d => d.PersonaId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("asignacion_dispositivo_persona_id_fkey");
        });

        modelBuilder.Entity<BalanceoApartadosHistorial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("balanceo_apartados_historial_pkey");

            entity.ToTable("balanceo_apartados_historial");

            entity.HasIndex(e => e.ClaveCnis, "idx_bah_clave");

            entity.HasIndex(e => new { e.ClaveCnis, e.Jurisdiccion }, "idx_bah_clave_jurisdiccion");

            entity.HasIndex(e => e.EjecucionId, "idx_bah_ejecucion");

            entity.HasIndex(e => e.Jurisdiccion, "idx_bah_jurisdiccion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CantidadApartada)
                .HasDefaultValue(0)
                .HasColumnName("cantidad_apartada");
            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(30)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.CluesAlmacen)
                .HasMaxLength(20)
                .HasColumnName("clues_almacen");
            entity.Property(e => e.CpmJurisdiccion)
                .HasDefaultValue(0)
                .HasColumnName("cpm_jurisdiccion");
            entity.Property(e => e.EjecucionId).HasColumnName("ejecucion_id");
            entity.Property(e => e.ExistenciaDisponibleBalanceo)
                .HasDefaultValue(0)
                .HasColumnName("existencia_disponible_balanceo");
            entity.Property(e => e.ExistenciaOriginal)
                .HasDefaultValue(0)
                .HasColumnName("existencia_original");
            entity.Property(e => e.FechaEjecucion)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_ejecucion");
            entity.Property(e => e.Jurisdiccion)
                .HasMaxLength(100)
                .HasColumnName("jurisdiccion");
            entity.Property(e => e.NombreAlmacen)
                .HasMaxLength(255)
                .HasColumnName("nombre_almacen");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
        });

        modelBuilder.Entity<BalanceoDetalladoFinal>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("balanceo_detallado_final");

            entity.Property(e => e.CantidadSugerida).HasColumnName("cantidad_sugerida");
            entity.Property(e => e.ClaveCnis).HasColumnName("clave_cnis");
            entity.Property(e => e.CluesDestino).HasColumnName("clues_destino");
            entity.Property(e => e.JurisdiccionAlmacen).HasColumnName("jurisdiccion_almacen");
            entity.Property(e => e.JurisdiccionDestino).HasColumnName("jurisdiccion_destino");
            entity.Property(e => e.NecesidadOriginal).HasColumnName("necesidad_original");
            entity.Property(e => e.NombreUnidadDestino).HasColumnName("nombre_unidad_destino");
            entity.Property(e => e.Prioridad).HasColumnName("prioridad");
        });

        modelBuilder.Entity<CatPerifericoTipo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cat_periferico_tipo_pkey");

            entity.ToTable("cat_periferico_tipo");

            entity.HasIndex(e => e.Nombre, "cat_periferico_tipo_nombre_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
        });

        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("citas_pkey");

            entity.ToTable("citas");

            entity.HasIndex(e => e.ClaveCnis, "idx_citas_clave");

            entity.HasIndex(e => new { e.ClaveCnis, e.FechaRecepcionLista }, "idx_citas_clave_recepcion_lista");

            entity.HasIndex(e => e.CluesDestino, "idx_citas_clues");

            entity.HasIndex(e => e.Ejercicio, "idx_citas_ejercicio");

            entity.HasIndex(e => e.FechaDeCita, "idx_citas_fecha_cita");

            entity.HasIndex(e => new { e.FechaDeCita, e.Id }, "idx_citas_fecha_id");

            entity.HasIndex(e => e.FechaLimiteDeEntrega, "idx_citas_limite");

            entity.HasIndex(e => e.Recibido, "idx_citas_recibido");

            entity.HasIndex(e => e.Estatus, "idx_citas_status");

            entity.HasIndex(e => e.TipoDeEntrega, "idx_citas_tipo_entrega");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlmacenHospitalQueRecibio)
                .HasMaxLength(100)
                .HasColumnName("almacen_hospital_que_recibio");
            entity.Property(e => e.AtrasoDias)
                .HasComputedColumnSql("\nCASE\n    WHEN ((fecha_limite_de_entrega IS NOT NULL) AND (arr_date_min(parse_ddmmyyyy_list((fecha_recepcion_almacen)::text)) IS NOT NULL)) THEN (arr_date_min(parse_ddmmyyyy_list((fecha_recepcion_almacen)::text)) - fecha_limite_de_entrega)\n    ELSE NULL::integer\nEND", true)
                .HasColumnName("atraso_dias");
            entity.Property(e => e.Caducidad)
                .HasMaxLength(2048)
                .HasColumnName("caducidad");
            entity.Property(e => e.Carga)
                .HasMaxLength(100)
                .HasColumnName("carga");
            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(100)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.CluesDestino)
                .HasMaxLength(100)
                .HasColumnName("clues_destino");
            entity.Property(e => e.Compra)
                .HasMaxLength(100)
                .HasColumnName("compra");
            entity.Property(e => e.Contrato)
                .HasMaxLength(100)
                .HasColumnName("contrato");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(2048)
                .HasColumnName("descripcion");
            entity.Property(e => e.Ejercicio).HasColumnName("ejercicio");
            entity.Property(e => e.Estatus)
                .HasMaxLength(100)
                .HasColumnName("estatus");
            entity.Property(e => e.Evidencia)
                .HasMaxLength(2048)
                .HasColumnName("evidencia");
            entity.Property(e => e.FechaDeCita).HasColumnName("fecha_de_cita");
            entity.Property(e => e.FechaEmision).HasColumnName("fecha_emision");
            entity.Property(e => e.FechaLimiteDeEntrega).HasColumnName("fecha_limite_de_entrega");
            entity.Property(e => e.FechaRecepcionAlmacen)
                .HasMaxLength(100)
                .HasColumnName("fecha_recepcion_almacen");
            entity.Property(e => e.FechaRecepcionLista)
                .HasComputedColumnSql("parse_ddmmyyyy_list((fecha_recepcion_almacen)::text)", true)
                .HasColumnName("fecha_recepcion_lista");
            entity.Property(e => e.FechaRecepcionMax)
                .HasComputedColumnSql("arr_date_max(parse_ddmmyyyy_list((fecha_recepcion_almacen)::text))", true)
                .HasColumnName("fecha_recepcion_max");
            entity.Property(e => e.FechaRecepcionMin)
                .HasComputedColumnSql("arr_date_min(parse_ddmmyyyy_list((fecha_recepcion_almacen)::text))", true)
                .HasColumnName("fecha_recepcion_min");
            entity.Property(e => e.FolioAbasto)
                .HasMaxLength(100)
                .HasColumnName("folio_abasto");
            entity.Property(e => e.FteFmto)
                .HasMaxLength(100)
                .HasColumnName("fte_fmto");
            entity.Property(e => e.GrupoTerapeutico)
                .HasMaxLength(100)
                .HasColumnName("grupo_terapeutico");
            entity.Property(e => e.Institucion)
                .HasMaxLength(50)
                .HasColumnName("institucion");
            entity.Property(e => e.Lote)
                .HasMaxLength(2048)
                .HasColumnName("lote");
            entity.Property(e => e.NoDePiezasEmitidas).HasColumnName("no_de_piezas_emitidas");
            entity.Property(e => e.NumeroDeRemision)
                .HasMaxLength(100)
                .HasColumnName("numero_de_remision");
            entity.Property(e => e.OrdenDeSuministro)
                .HasMaxLength(100)
                .HasColumnName("orden_de_suministro");
            entity.Property(e => e.PrecioUnitario).HasColumnName("precio_unitario");
            entity.Property(e => e.Procedimiento)
                .HasMaxLength(100)
                .HasColumnName("procedimiento");
            entity.Property(e => e.Proveedor)
                .HasMaxLength(255)
                .HasColumnName("proveedor");
            entity.Property(e => e.PzasRecibidasPorLaEntidad).HasColumnName("pzas_recibidas_por_la_entidad");
            entity.Property(e => e.Recibido).HasColumnName("recibido");
            entity.Property(e => e.TipoDeEntrega)
                .HasMaxLength(100)
                .HasColumnName("tipo_de_entrega");
            entity.Property(e => e.TipoDeInsumo)
                .HasMaxLength(100)
                .HasColumnName("tipo_de_insumo");
            entity.Property(e => e.TipoDeRed)
                .HasMaxLength(100)
                .HasColumnName("tipo_de_red");
            entity.Property(e => e.Unidad)
                .HasMaxLength(255)
                .HasColumnName("unidad");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Cpm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cpm_pkey");

            entity.ToTable("cpm");

            entity.HasIndex(e => new { e.UnidadMedicaId, e.ClaveCnis }, "cpm_unq_um_clave").IsUnique();

            entity.HasIndex(e => e.ClaveCnis, "idx_cpm_clave");

            entity.HasIndex(e => e.UnidadMedicaId, "idx_cpm_um");

            entity.HasIndex(e => new { e.UnidadMedicaId, e.ClaveCnis }, "uq_cpm_unidad_clave").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(20)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.Cpm1).HasColumnName("cpm");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.Fuente)
                .HasMaxLength(255)
                .HasColumnName("fuente");
            entity.Property(e => e.UnidadMedicaId).HasColumnName("unidad_medica_id");

            entity.HasOne(d => d.UnidadMedica).WithMany(p => p.Cpms)
                .HasForeignKey(d => d.UnidadMedicaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cpm_um_fk");
        });

        modelBuilder.Entity<CpmReal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cpm_real_pkey");

            entity.ToTable("cpm_real");

            entity.HasIndex(e => new { e.Cluesimb, e.ClaveCnis }, "cpm_real_unq_um_clave").IsUnique();

            entity.HasIndex(e => e.ClaveCnis, "idx_cpm_real_clave");

            entity.HasIndex(e => e.Cluesimb, "idx_cpm_real_um");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(20)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.ClavesDeKit).HasColumnName("claves_de_kit");
            entity.Property(e => e.Cluesimb)
                .HasMaxLength(20)
                .HasColumnName("cluesimb");
            entity.Property(e => e.Cpm).HasColumnName("cpm");
            entity.Property(e => e.TemporalidadDeEntrega).HasColumnName("temporalidad_de_entrega");
            entity.Property(e => e.UnidadOncologica55).HasColumnName("unidad_oncologica_55");
        });

        modelBuilder.Entity<Dispositivo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("dispositivo_pkey");

            entity.ToTable("dispositivo");

            entity.HasIndex(e => e.Marca, "idx_dispositivo_marca_trgm")
                .HasMethod("gin")
                .HasOperators(new[] { "gin_trgm_ops" });

            entity.HasIndex(e => e.Modelo, "idx_dispositivo_modelo_trgm")
                .HasMethod("gin")
                .HasOperators(new[] { "gin_trgm_ops" });

            entity.HasIndex(e => e.Serial, "idx_dispositivo_serial_trgm")
                .HasMethod("gin")
                .HasOperators(new[] { "gin_trgm_ops" });

            entity.HasIndex(e => e.TipoDispositivoId, "idx_dispositivo_tipo");

            entity.HasIndex(e => e.UnidadMedicaId, "idx_dispositivo_unidad");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.AnydeskId)
                .HasMaxLength(20)
                .HasColumnName("anydesk_id");
            entity.Property(e => e.Conexion)
                .HasMaxLength(10)
                .HasColumnName("conexion");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("creado_en");
            entity.Property(e => e.Ip)
                .HasMaxLength(15)
                .HasColumnName("ip");
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .HasColumnName("marca");
            entity.Property(e => e.Modelo)
                .HasMaxLength(100)
                .HasColumnName("modelo");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.RustdeskId)
                .HasMaxLength(20)
                .HasColumnName("rustdesk_id");
            entity.Property(e => e.Serial)
                .HasMaxLength(100)
                .HasColumnName("serial");
            entity.Property(e => e.SupremoId)
                .HasMaxLength(20)
                .HasColumnName("supremo_id");
            entity.Property(e => e.TipoDispositivoId).HasColumnName("tipo_dispositivo_id");
            entity.Property(e => e.UnidadMedicaId).HasColumnName("unidad_medica_id");

            entity.HasOne(d => d.TipoDispositivo).WithMany(p => p.Dispositivos)
                .HasForeignKey(d => d.TipoDispositivoId)
                .HasConstraintName("dispositivo_tipo_dispositivo_id_fkey");

            entity.HasOne(d => d.UnidadMedica).WithMany(p => p.Dispositivos)
                .HasForeignKey(d => d.UnidadMedicaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("dispositivo_unidad_medica_id_fkey");
        });

        modelBuilder.Entity<Entradum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("entrada_pkey");

            entity.ToTable("entrada");

            entity.HasIndex(e => new { e.ClaveCnis, e.Fecha }, "idx_ent_clave_fecha");

            entity.HasIndex(e => e.UnidadDestinoId, "idx_ent_umdest");

            entity.HasIndex(e => new { e.Folio, e.Fecha }, "idx_entrada_folio_fecha").IsDescending(false, true);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Anio).HasColumnName("anio");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.CantidadExistencia).HasColumnName("cantidad_existencia");
            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(50)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.Costo).HasColumnName("costo");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("creado_en");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.DescripcionExtra).HasColumnName("descripcion_extra");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.FechaCaducidad).HasColumnName("fecha_caducidad");
            entity.Property(e => e.Folio)
                .HasMaxLength(50)
                .HasColumnName("folio");
            entity.Property(e => e.Lote)
                .HasMaxLength(150)
                .HasColumnName("lote");
            entity.Property(e => e.NumFactura)
                .HasMaxLength(150)
                .HasColumnName("num_factura");
            entity.Property(e => e.NumRemision)
                .HasMaxLength(150)
                .HasColumnName("num_remision");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.Proveedor).HasColumnName("proveedor");
            entity.Property(e => e.TipoDocumento).HasColumnName("tipo_documento");
            entity.Property(e => e.UnidadDestinoId).HasColumnName("unidad_destino_id");
            entity.Property(e => e.UnidadDestinoTexto).HasColumnName("unidad_destino_texto");

            entity.HasOne(d => d.UnidadDestino).WithMany(p => p.Entrada)
                .HasForeignKey(d => d.UnidadDestinoId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("entrada_unidad_destino_id_fkey");
        });

        modelBuilder.Entity<EstadoDispositivo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("estado_dispositivo_pkey");

            entity.ToTable("estado_dispositivo");

            entity.HasIndex(e => e.Nombre, "estado_dispositivo_nombre_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<FactoresConversion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("factores_conversion_pkey");

            entity.ToTable("factores_conversion");

            entity.HasIndex(e => e.Clave, "idx_fc_clave");

            entity.HasIndex(e => e.Cluesimb, "idx_fc_clues");

            entity.HasIndex(e => new { e.Clave, e.Cluesimb }, "uq_factores_conversion_clave_clues").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CantidadFc).HasColumnName("cantidad_fc");
            entity.Property(e => e.Clave)
                .HasMaxLength(20)
                .HasColumnName("clave");
            entity.Property(e => e.Cluesimb)
                .HasMaxLength(20)
                .HasColumnName("cluesimb");
            entity.Property(e => e.DescPartida).HasColumnName("desc_partida");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.EnDispensacion)
                .HasDefaultValue((short)0)
                .HasColumnName("en_dispensacion");
            entity.Property(e => e.Partida)
                .HasMaxLength(10)
                .HasColumnName("partida");
            entity.Property(e => e.PresentacionDisp)
                .HasMaxLength(50)
                .HasColumnName("presentacion_disp");
            entity.Property(e => e.PresentacionPres)
                .HasMaxLength(100)
                .HasColumnName("presentacion_pres");
            entity.Property(e => e.SasClave)
                .HasMaxLength(10)
                .HasColumnName("sas_clave");
        });

        modelBuilder.Entity<FeatureFlag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("feature_flags_pkey");

            entity.ToTable("feature_flags");

            entity.HasIndex(e => e.FlagKey, "idx_feature_flags_key");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FlagKey).HasColumnName("flag_key");
            entity.Property(e => e.Scope).HasColumnName("scope");
            entity.Property(e => e.ScopeId).HasColumnName("scope_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.ValueJson)
                .HasColumnType("jsonb")
                .HasColumnName("value_json");
        });

        modelBuilder.Entity<Homologo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("homologos_pkey");

            entity.ToTable("homologos", tb => tb.HasComment("Tabla de productos homólogos/sustitutos con sus factores de conversión"));

            entity.HasIndex(e => e.Clave, "idx_homologos_clave");

            entity.HasIndex(e => new { e.Clave, e.Sustituto }, "idx_homologos_clave_sustituto");

            entity.HasIndex(e => e.Sustituto, "idx_homologos_sustituto");

            entity.Property(e => e.Id)
                .HasComment("Identificador único autoincrementable")
                .HasColumnName("id");
            entity.Property(e => e.Clave)
                .HasMaxLength(20)
                .HasComment("Código del producto original")
                .HasColumnName("clave");
            entity.Property(e => e.Factor)
                .HasPrecision(15, 10)
                .HasComment("Factor de conversión entre productos")
                .HasColumnName("factor");
            entity.Property(e => e.Sustituto)
                .HasMaxLength(20)
                .HasComment("Código del producto sustituto")
                .HasColumnName("sustituto");
        });

        modelBuilder.Entity<InventarioInicial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("inventario_inicial_pkey");

            entity.ToTable("inventario_inicial");

            entity.HasIndex(e => new { e.ClaveCnis, e.Anio }, "idx_invini_clave_anio");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Anio).HasColumnName("anio");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(50)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.Costo).HasColumnName("costo");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("creado_en");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.FechaCaducidad).HasColumnName("fecha_caducidad");
            entity.Property(e => e.Lote)
                .HasMaxLength(150)
                .HasColumnName("lote");
            entity.Property(e => e.Partida).HasColumnName("partida");
            entity.Property(e => e.Tipo).HasColumnName("tipo");
            entity.Property(e => e.UnidadId).HasColumnName("unidad_id");
            entity.Property(e => e.UnidadTexto).HasColumnName("unidad_texto");

            entity.HasOne(d => d.Unidad).WithMany(p => p.InventarioInicials)
                .HasForeignKey(d => d.UnidadId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("inventario_inicial_unidad_id_fkey");
        });

        modelBuilder.Entity<Kit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("kit_pkey");

            entity.ToTable("kit");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Codigo).HasColumnName("codigo");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
        });

        modelBuilder.Entity<KitClave>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("kit_clave_pkey");

            entity.ToTable("kit_clave");

            entity.HasIndex(e => e.Clave, "idx_kit_clave_clave");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Aplica)
                .HasDefaultValue(true)
                .HasColumnName("aplica");
            entity.Property(e => e.Clave).HasColumnName("clave");
            entity.Property(e => e.KitId).HasColumnName("kit_id");

            entity.HasOne(d => d.Kit).WithMany(p => p.KitClaves)
                .HasForeignKey(d => d.KitId)
                .HasConstraintName("kit_clave_kit_id_fkey");
        });

        modelBuilder.Entity<Localidad>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("localidad_pkey");

            entity.ToTable("localidad");

            entity.HasIndex(e => e.MunicipioId, "ix_localidad_municipio_id");

            entity.HasIndex(e => new { e.NombreLocalidad, e.MunicipioId }, "localidad_nombre_localidad_municipio_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MunicipioId).HasColumnName("municipio_id");
            entity.Property(e => e.NombreLocalidad)
                .HasMaxLength(150)
                .HasColumnName("nombre_localidad");

            entity.HasOne(d => d.Municipio).WithMany(p => p.Localidads)
                .HasForeignKey(d => d.MunicipioId)
                .HasConstraintName("localidad_municipio_id_fkey");
        });

        modelBuilder.Entity<LogEjecucionesBalanceo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("log_ejecuciones_balanceo_pkey");

            entity.ToTable("log_ejecuciones_balanceo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClavesProcesadas).HasColumnName("claves_procesadas");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.FechaFin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_inicio");
            entity.Property(e => e.TotalClaves).HasColumnName("total_claves");
        });

        modelBuilder.Entity<MonitorEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("monitor_pkey");

            entity.ToTable("monitor");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DispositivoId).HasColumnName("dispositivo_id");
            entity.Property(e => e.EsPrincipal)
                .HasDefaultValue(false)
                .HasColumnName("es_principal");
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .HasDefaultValueSql("'HP'::character varying")
                .HasColumnName("marca");
            entity.Property(e => e.Modelo)
                .HasMaxLength(100)
                .HasColumnName("modelo");
            entity.Property(e => e.Serial)
                .HasMaxLength(100)
                .HasColumnName("serial");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<Municipio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("municipio_pkey");

            entity.ToTable("municipio");

            entity.HasIndex(e => e.NombreMunicipio, "municipio_nombre_municipio_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NombreMunicipio)
                .HasMaxLength(100)
                .HasColumnName("nombre_municipio");
        });

        modelBuilder.Entity<OncoClafe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("onco_claves_pkey");

            entity.ToTable("onco_claves");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(50)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.Cluesimb)
                .HasMaxLength(50)
                .HasColumnName("cluesimb");
        });

        modelBuilder.Entity<OncoClavesBase>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("onco_claves_base");

            entity.HasIndex(e => new { e.Cluesimb, e.ClaveCnis }, "idx_onco_claves_base_clues_clave");

            entity.HasIndex(e => new { e.Cluesimb, e.ClaveCnis }, "uq_onco_claves_base_clues_clave").IsUnique();

            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(50)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.Cluesimb)
                .HasMaxLength(50)
                .HasColumnName("cluesimb");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(null, null, 0L, null, null, null)
                .HasColumnName("id");
        });

        modelBuilder.Entity<OncoClase>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("onco_clases_pkey");
            entity.ToTable("onco_clases", "public");
            entity.HasIndex(e => e.Codigo, "ux_onco_clases_codigo").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn().HasColumnName("id");
            entity.Property(e => e.Codigo).HasMaxLength(10).HasColumnName("codigo");
            entity.Property(e => e.Nombre).HasMaxLength(100).HasColumnName("nombre");
            entity.Property(e => e.Descripcion).HasColumnType("text").HasColumnName("descripcion");
            entity.Property(e => e.StockFactor).HasPrecision(6, 2).HasColumnName("stock_factor");
            entity.Property(e => e.Activo).HasDefaultValue(true).HasColumnName("activo");
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("now()").HasColumnName("creado_en");
            entity.Property(e => e.ActualizadoEn).HasDefaultValueSql("now()").HasColumnName("actualizado_en");
        });

        modelBuilder.Entity<OncoSubclase>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("onco_subclases_pkey");
            entity.ToTable("onco_subclases", "public");
            entity.HasIndex(e => e.Codigo, "ux_onco_subclases_codigo").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn().HasColumnName("id");
            entity.Property(e => e.Codigo).HasMaxLength(20).HasColumnName("codigo");
            entity.Property(e => e.Nombre).HasMaxLength(150).HasColumnName("nombre");
            entity.Property(e => e.Descripcion).HasColumnType("text").HasColumnName("descripcion");
            entity.Property(e => e.Activo).HasDefaultValue(true).HasColumnName("activo");
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("now()").HasColumnName("creado_en");
            entity.Property(e => e.ActualizadoEn).HasDefaultValueSql("now()").HasColumnName("actualizado_en");
        });

        modelBuilder.Entity<OncoUnidade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("onco_unidades_pkey");

            entity.ToTable("onco_unidades");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Cluesimb)
                .HasMaxLength(50)
                .HasColumnName("cluesimb");
        });

        modelBuilder.Entity<Periferico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("periferico_pkey");

            entity.ToTable("periferico");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DispositivoId).HasColumnName("dispositivo_id");
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .HasDefaultValueSql("'HP'::character varying")
                .HasColumnName("marca");
            entity.Property(e => e.Modelo)
                .HasMaxLength(100)
                .HasColumnName("modelo");
            entity.Property(e => e.Serial)
                .HasMaxLength(100)
                .HasColumnName("serial");
            entity.Property(e => e.TipoId).HasColumnName("tipo_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<PermBalanceoDetalladoFinal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("perm_balanceo_detallado_final_pkey");

            entity.ToTable("perm_balanceo_detallado_final");

            entity.HasIndex(e => e.EjecucionId, "idx_perm_detallado_ejecucion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CantidadSugerida).HasColumnName("cantidad_sugerida");
            entity.Property(e => e.ClaveCnis).HasColumnName("clave_cnis");
            entity.Property(e => e.CluesDestino).HasColumnName("clues_destino");
            entity.Property(e => e.EjecucionId).HasColumnName("ejecucion_id");
            entity.Property(e => e.FechaEjecucion)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_ejecucion");
            entity.Property(e => e.JurisdiccionAlmacen).HasColumnName("jurisdiccion_almacen");
            entity.Property(e => e.JurisdiccionDestino).HasColumnName("jurisdiccion_destino");
            entity.Property(e => e.NecesidadOriginal).HasColumnName("necesidad_original");
            entity.Property(e => e.NombreUnidadDestino).HasColumnName("nombre_unidad_destino");
            entity.Property(e => e.Prioridad).HasColumnName("prioridad");

            entity.HasOne(d => d.Ejecucion).WithMany(p => p.PermBalanceoDetalladoFinals)
                .HasForeignKey(d => d.EjecucionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("perm_balanceo_detallado_final_ejecucion_id_fkey");
        });

        modelBuilder.Entity<PermBalanceoResultado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("perm_balanceo_resultados_pkey");

            entity.ToTable("perm_balanceo_resultados");

            entity.HasIndex(e => e.EjecucionId, "idx_perm_resultados_ejecucion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CantidadTransferir).HasColumnName("cantidad_transferir");
            entity.Property(e => e.ClaveCnis).HasColumnName("clave_cnis");
            entity.Property(e => e.EjecucionId).HasColumnName("ejecucion_id");
            entity.Property(e => e.ExistenciaOriginal).HasColumnName("existencia_original");
            entity.Property(e => e.FechaEjecucion)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_ejecucion");
            entity.Property(e => e.JurisdiccionDestino).HasColumnName("jurisdiccion_destino");
            entity.Property(e => e.JurisdiccionOrigen).HasColumnName("jurisdiccion_origen");
            entity.Property(e => e.NecesidadDestino).HasColumnName("necesidad_destino");

            entity.HasOne(d => d.Ejecucion).WithMany(p => p.PermBalanceoResultados)
                .HasForeignKey(d => d.EjecucionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("perm_balanceo_resultados_ejecucion_id_fkey");
        });

        modelBuilder.Entity<PermResumenAlmacenesFinal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("perm_resumen_almacenes_final_pkey");

            entity.ToTable("perm_resumen_almacenes_final");

            entity.HasIndex(e => e.EjecucionId, "idx_perm_resumen_ejecucion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClaveCnis).HasColumnName("clave_cnis");
            entity.Property(e => e.EjecucionId).HasColumnName("ejecucion_id");
            entity.Property(e => e.FechaEjecucion)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_ejecucion");
            entity.Property(e => e.InstruccionesDetalladas).HasColumnName("instrucciones_detalladas");
            entity.Property(e => e.JurisdiccionAlmacen).HasColumnName("jurisdiccion_almacen");
            entity.Property(e => e.JurisdiccionDestino).HasColumnName("jurisdiccion_destino");
            entity.Property(e => e.TotalPiezas).HasColumnName("total_piezas");
            entity.Property(e => e.TotalUnidades).HasColumnName("total_unidades");

            entity.HasOne(d => d.Ejecucion).WithMany(p => p.PermResumenAlmacenesFinals)
                .HasForeignKey(d => d.EjecucionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("perm_resumen_almacenes_final_ejecucion_id_fkey");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("persona_pkey");

            entity.ToTable("persona");

            entity.HasIndex(e => e.NombreCompleto, "idx_persona_nombre");

            entity.HasIndex(e => e.NombreCompleto, "idx_persona_nombre_trgm")
                .HasMethod("gin")
                .HasOperators(new[] { "gin_trgm_ops" });

            entity.HasIndex(e => new { e.Activo, e.UnidadMedicaId }, "ix_persona_activo_unidad");

            entity.HasIndex(e => e.UnidadMedicaId, "persona_unidad_medica_id_idx");

            entity.HasIndex(e => e.Curp, "uq_persona_curp")
                .IsUnique()
                .HasFilter("(curp IS NOT NULL)");

            entity.HasIndex(e => e.Rfc, "uq_persona_rfc")
                .IsUnique()
                .HasFilter("(rfc IS NOT NULL)");

            entity.HasIndex(e => e.UserId, "uq_persona_user_id")
                .IsUnique()
                .HasFilter("(user_id IS NOT NULL)");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(150)
                .HasColumnName("apellidos");
            entity.Property(e => e.Cargo)
                .HasMaxLength(100)
                .HasColumnName("cargo");
            entity.Property(e => e.CorreoPrincipal).HasColumnName("correo_principal");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("creado_en");
            entity.Property(e => e.Curp)
                .HasMaxLength(18)
                .HasColumnName("curp");
            entity.Property(e => e.FechaBaja).HasColumnName("fecha_baja");
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(150)
                .HasColumnName("nombre_completo");
            entity.Property(e => e.Nombres)
                .HasMaxLength(150)
                .HasColumnName("nombres");
            entity.Property(e => e.Rfc)
                .HasMaxLength(13)
                .HasColumnName("rfc");
            entity.Property(e => e.UnidadMedicaId).HasColumnName("unidad_medica_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasOne(d => d.UnidadMedica).WithMany(p => p.Personas)
                .HasForeignKey(d => d.UnidadMedicaId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("persona_unidad_medica_id_fkey");
        });

        modelBuilder.Entity<PersonaCorreo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("persona_correo_pkey");

            entity.ToTable("persona_correo");

            entity.HasIndex(e => e.Correo, "idx_persona_correo_trgm")
                .HasMethod("gin")
                .HasOperators(new[] { "gin_trgm_ops" });

            entity.HasIndex(e => e.PersonaId, "ix_pc_persona_activo").HasFilter("(activo IS TRUE)");

            entity.HasIndex(e => e.PersonaId, "ix_pc_persona_principal_activo").HasFilter("((activo IS TRUE) AND (es_principal IS TRUE))");

            entity.HasIndex(e => e.PersonaId, "ix_persona_correo_persona_id");

            entity.HasIndex(e => e.PersonaId, "uq_persona_correo_1principal_active")
                .IsUnique()
                .HasFilter("((activo IS TRUE) AND (es_principal IS TRUE))");

            entity.HasIndex(e => e.PersonaId, "uq_persona_correo_principal_por_persona")
                .IsUnique()
                .HasFilter("(es_principal IS TRUE)");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Correo).HasColumnName("correo");
            entity.Property(e => e.EsPrincipal)
                .HasDefaultValue(false)
                .HasColumnName("es_principal");
            entity.Property(e => e.PersonaId).HasColumnName("persona_id");

            entity.HasOne(d => d.Persona).WithOne(p => p.PersonaCorreo)
                .HasForeignKey<PersonaCorreo>(d => d.PersonaId)
                .HasConstraintName("persona_correo_persona_id_fkey");
        });

        modelBuilder.Entity<RadarEvento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("radar_eventos_pkey");

            entity.ToTable("radar_eventos");

            entity.HasIndex(e => e.Clues, "idx_radar_eventos_clues");

            entity.HasIndex(e => e.Estado, "idx_radar_eventos_estado");

            entity.HasIndex(e => e.FechaEvento, "idx_radar_eventos_fecha").IsDescending();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Clues)
                .HasMaxLength(20)
                .HasColumnName("clues");
            entity.Property(e => e.CreadoPor)
                .HasDefaultValueSql("'sistema'::text")
                .HasColumnName("creado_por");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValueSql("'abierto'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.FechaEvento)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("fecha_evento");
            entity.Property(e => e.FechaReferencia).HasColumnName("fecha_referencia");
            entity.Property(e => e.Motivo).HasColumnName("motivo");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.TipoInsumo)
                .HasMaxLength(80)
                .HasColumnName("tipo_insumo");
            entity.Property(e => e.UnidadNombre).HasColumnName("unidad_nombre");
        });

        modelBuilder.Entity<RadarEventoClafe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("radar_evento_claves_pkey");

            entity.ToTable("radar_evento_claves");

            entity.HasIndex(e => e.ClaveCnis, "idx_radar_evento_claves_clave");

            entity.HasIndex(e => e.EventoId, "idx_radar_evento_claves_evento");

            entity.HasIndex(e => e.NivelRiesgo, "idx_radar_evento_claves_riesgo");

            entity.HasIndex(e => new { e.EventoId, e.ClaveCnis }, "uq_radar_evento_clave").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CitasPendientes).HasColumnName("citas_pendientes");
            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(20)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.ConsumoPromedio).HasColumnName("consumo_promedio");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.DiasCobertura).HasColumnName("dias_cobertura");
            entity.Property(e => e.Entradas30d).HasColumnName("entradas_30d");
            entity.Property(e => e.EventoId).HasColumnName("evento_id");
            entity.Property(e => e.ExistenciaActual).HasColumnName("existencia_actual");
            entity.Property(e => e.Flags)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("flags");
            entity.Property(e => e.MovimientosRecientes)
                .HasDefaultValue(0)
                .HasColumnName("movimientos_recientes");
            entity.Property(e => e.NivelRiesgo)
                .HasMaxLength(20)
                .HasDefaultValueSql("'BAJO'::character varying")
                .HasColumnName("nivel_riesgo");
            entity.Property(e => e.RecalculatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("recalculated_at");
            entity.Property(e => e.Salidas30d).HasColumnName("salidas_30d");
            entity.Property(e => e.Solicitado30d).HasColumnName("solicitado_30d");
            entity.Property(e => e.Traspasos30d).HasColumnName("traspasos_30d");

            entity.HasOne(d => d.Evento).WithMany(p => p.RadarEventoClaves)
                .HasForeignKey(d => d.EventoId)
                .HasConstraintName("radar_evento_claves_evento_id_fkey");
        });

        modelBuilder.Entity<ResumenAlmacenesFinal>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("resumen_almacenes_final");

            entity.Property(e => e.ClaveCnis).HasColumnName("clave_cnis");
            entity.Property(e => e.InstruccionesDetalladas).HasColumnName("instrucciones_detalladas");
            entity.Property(e => e.JurisdiccionAlmacen).HasColumnName("jurisdiccion_almacen");
            entity.Property(e => e.JurisdiccionDestino).HasColumnName("jurisdiccion_destino");
            entity.Property(e => e.TotalPiezas).HasColumnName("total_piezas");
            entity.Property(e => e.TotalUnidades).HasColumnName("total_unidades");
        });

        modelBuilder.Entity<Salidum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("salida_pkey");

            entity.ToTable("salida");

            entity.HasIndex(e => new { e.ClaveCnis, e.FechaEntregado }, "idx_sal_clave_fecha");

            entity.HasIndex(e => new { e.UnidadOrigenId, e.UnidadDestinoId }, "idx_sal_umori_umdest");

            entity.HasIndex(e => e.ClaveCnis, "idx_salida_clave");

            entity.HasIndex(e => e.UnidadDestinoId, "idx_salida_destino");

            entity.HasIndex(e => new { e.UnidadDestinoId, e.FechaEntregado }, "idx_salida_destino_fecha");

            entity.HasIndex(e => new { e.UnidadOrigenId, e.FechaEntregado, e.Id }, "idx_salida_origen_fecha").IsDescending(false, true, true);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(50)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("creado_en");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.FechaCaducidad).HasColumnName("fecha_caducidad");
            entity.Property(e => e.FechaEntregado).HasColumnName("fecha_entregado");
            entity.Property(e => e.Folio)
                .HasMaxLength(150)
                .HasColumnName("folio");
            entity.Property(e => e.FolioExtra)
                .HasMaxLength(150)
                .HasColumnName("folio_extra");
            entity.Property(e => e.Lote)
                .HasMaxLength(150)
                .HasColumnName("lote");
            entity.Property(e => e.Movto)
                .HasMaxLength(50)
                .HasColumnName("movto");
            entity.Property(e => e.Programa).HasColumnName("programa");
            entity.Property(e => e.ProgramaExtra).HasColumnName("programa_extra");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .HasColumnName("tipo");
            entity.Property(e => e.Total).HasColumnName("total");
            entity.Property(e => e.UnidadDestinoId).HasColumnName("unidad_destino_id");
            entity.Property(e => e.UnidadDestinoTexto).HasColumnName("unidad_destino_texto");
            entity.Property(e => e.UnidadOrigenId).HasColumnName("unidad_origen_id");
            entity.Property(e => e.UnidadOrigenTexto).HasColumnName("unidad_origen_texto");

            entity.HasOne(d => d.UnidadDestino).WithMany(p => p.SalidumUnidadDestinos)
                .HasForeignKey(d => d.UnidadDestinoId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("salida_unidad_destino_id_fkey");

            entity.HasOne(d => d.UnidadOrigen).WithMany(p => p.SalidumUnidadOrigens)
                .HasForeignKey(d => d.UnidadOrigenId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("salida_unidad_origen_id_fkey");
        });

        modelBuilder.Entity<SolicitudBitacora>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("solicitud_bitacora_pkey");

            entity.ToTable("solicitud_bitacora");

            entity.HasIndex(e => new { e.Cluesimb, e.CreatedDay }, "idx_solic_bitacora_clues_day").IsDescending(false, true);

            entity.HasIndex(e => e.CreatedDay, "idx_solic_bitacora_day").IsDescending();

            entity.HasIndex(e => e.CreatedAt, "ix_solicitud_bitacora_created_at").IsDescending();

            entity.HasIndex(e => new { e.Cluesimb, e.CreatedDay, e.PayloadHash }, "ux_solicitud_bitacora_dedupe").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Cluesimb).HasColumnName("cluesimb");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedDay)
                .HasComputedColumnSql("((created_at AT TIME ZONE 'UTC'::text))::date", true)
                .HasColumnName("created_day");
            entity.Property(e => e.ExportKind)
                .HasDefaultValueSql("'raw'::text")
                .HasColumnName("export_kind");
            entity.Property(e => e.PayloadHash).HasColumnName("payload_hash");
            entity.Property(e => e.PeriodoTexto).HasColumnName("periodo_texto");
            entity.Property(e => e.TipoPedido).HasColumnName("tipo_pedido");
            entity.Property(e => e.TiposInsumo).HasColumnName("tipos_insumo");
            entity.Property(e => e.TotalPiezas).HasColumnName("total_piezas");
            entity.Property(e => e.TotalRenglones).HasColumnName("total_renglones");
        });

        modelBuilder.Entity<SolicitudBitacoraDetalle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("solicitud_bitacora_detalle_pkey");

            entity.ToTable("solicitud_bitacora_detalle");

            entity.HasIndex(e => e.SolicitudId, "idx_solic_det_solicitud");

            entity.HasIndex(e => new { e.SolicitudId, e.Clave }, "idx_solic_det_solicitud_clave");

            entity.HasIndex(e => e.Clave, "ix_solicitud_detalle_clave");

            entity.HasIndex(e => e.SolicitudId, "ix_solicitud_detalle_solicitud");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Clave).HasColumnName("clave");
            entity.Property(e => e.SolicitudId).HasColumnName("solicitud_id");
            entity.Property(e => e.UnidadMedida).HasColumnName("unidad_medida");

            entity.HasOne(d => d.Solicitud).WithMany(p => p.SolicitudBitacoraDetalles)
                .HasForeignKey(d => d.SolicitudId)
                .HasConstraintName("solicitud_bitacora_detalle_solicitud_id_fkey");
        });

        modelBuilder.Entity<TipoDispositivo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tipo_dispositivo_pkey");

            entity.ToTable("tipo_dispositivo");

            entity.HasIndex(e => e.Nombre, "tipo_dispositivo_nombre_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TipoUnidad>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tipo_unidad_pkey");

            entity.ToTable("tipo_unidad");

            entity.HasIndex(e => e.NombreTipo, "tipo_unidad_nombre_tipo_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NombreTipo)
                .HasMaxLength(100)
                .HasColumnName("nombre_tipo");
        });

        modelBuilder.Entity<TmpExistencia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tmp_existencias_pkey");

            entity.ToTable("tmp_existencias");

            entity.HasIndex(e => e.ClaveCnis, "idx_tmp_existencias_clave");

            entity.HasIndex(e => e.Cluessa, "idx_tmp_existencias_cluessa");

            entity.HasIndex(e => e.Fuente, "idx_tmp_existencias_fuente");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AliasSas)
                .HasMaxLength(150)
                .HasColumnName("alias_sas");
            entity.Property(e => e.CargadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("cargado_en");
            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(50)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.Cluesimb)
                .HasMaxLength(20)
                .HasColumnName("cluesimb");
            entity.Property(e => e.Cluessa)
                .HasMaxLength(20)
                .HasColumnName("cluessa");
            entity.Property(e => e.Existencia).HasColumnName("existencia");
            entity.Property(e => e.FechaCaducidad).HasColumnName("fecha_caducidad");
            entity.Property(e => e.Fuente).HasColumnName("fuente");
            entity.Property(e => e.Lote)
                .HasMaxLength(150)
                .HasColumnName("lote");
        });

        modelBuilder.Entity<Traspaso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("traspaso_pkey");

            entity.ToTable("traspaso");

            entity.HasIndex(e => new { e.ClaveCnis, e.FechaRecepcion }, "idx_tra_clave_fecha");

            entity.HasIndex(e => new { e.UnidadOrigenId, e.UnidadDestinoId }, "idx_tra_umori_umdest");

            entity.HasIndex(e => e.ClaveCnis, "idx_traspaso_clave");

            entity.HasIndex(e => new { e.UnidadDestinoId, e.FechaRecepcion }, "idx_traspaso_destino_fecha");

            entity.HasIndex(e => new { e.Folio, e.FechaRecepcion }, "idx_traspaso_folio_fecha").IsDescending(false, true);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(50)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("creado_en");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.FechaCaducidad).HasColumnName("fecha_caducidad");
            entity.Property(e => e.FechaRecepcion).HasColumnName("fecha_recepcion");
            entity.Property(e => e.Folio)
                .HasMaxLength(50)
                .HasColumnName("folio");
            entity.Property(e => e.Lote)
                .HasMaxLength(150)
                .HasColumnName("lote");
            entity.Property(e => e.Partida)
                .HasMaxLength(150)
                .HasColumnName("partida");
            entity.Property(e => e.Total).HasColumnName("total");
            entity.Property(e => e.UnidadDestinoId).HasColumnName("unidad_destino_id");
            entity.Property(e => e.UnidadDestinoTexto).HasColumnName("unidad_destino_texto");
            entity.Property(e => e.UnidadOrigenId).HasColumnName("unidad_origen_id");
            entity.Property(e => e.UnidadOrigenTexto).HasColumnName("unidad_origen_texto");

            entity.HasOne(d => d.UnidadDestino).WithMany(p => p.TraspasoUnidadDestinos)
                .HasForeignKey(d => d.UnidadDestinoId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("traspaso_unidad_destino_id_fkey");

            entity.HasOne(d => d.UnidadOrigen).WithMany(p => p.TraspasoUnidadOrigens)
                .HasForeignKey(d => d.UnidadOrigenId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("traspaso_unidad_origen_id_fkey");
        });

        modelBuilder.Entity<UnidadMedica>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("unidad_medica_pkey");

            entity.ToTable("unidad_medica");

            entity.HasIndex(e => e.Nombre, "idx_um_nombre_trgm")
                .HasMethod("gin")
                .HasOperators(new[] { "gin_trgm_ops" });

            entity.HasIndex(e => e.LocalidadId, "idx_unidad_medica_localidad");

            entity.HasIndex(e => e.Nombre, "idx_unidad_medica_nombre_trgm")
                .HasMethod("gin")
                .HasOperators(new[] { "gin_trgm_ops" });

            entity.HasIndex(e => e.TipoUnidadId, "idx_unidad_medica_tipo");

            entity.HasIndex(e => e.LocalidadId, "ix_unidad_medica_localidad_id");

            entity.HasIndex(e => e.TipoUnidadId, "ix_unidad_medica_tipo_unidad_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Cluesimb)
                .HasMaxLength(20)
                .HasColumnName("cluesimb");
            entity.Property(e => e.Cluessa)
                .HasMaxLength(20)
                .HasColumnName("cluessa");
            entity.Property(e => e.Direccion).HasColumnName("direccion");
            entity.Property(e => e.EstratoUnidad)
                .HasMaxLength(10)
                .HasColumnName("estrato_unidad");
            entity.Property(e => e.Latitud)
                .HasPrecision(10, 6)
                .HasColumnName("latitud");
            entity.Property(e => e.LocalidadId).HasColumnName("localidad_id");
            entity.Property(e => e.Longitud)
                .HasPrecision(10, 6)
                .HasColumnName("longitud");
            entity.Property(e => e.NivelAtencion)
                .HasMaxLength(30)
                .HasColumnName("nivel_atencion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .HasColumnName("nombre");
            entity.Property(e => e.TipoUnidadId).HasColumnName("tipo_unidad_id");

            entity.HasOne(d => d.Localidad).WithMany(p => p.UnidadMedicas)
                .HasForeignKey(d => d.LocalidadId)
                .HasConstraintName("unidad_medica_localidad_id_fkey");

            entity.HasOne(d => d.TipoUnidad).WithMany(p => p.UnidadMedicas)
                .HasForeignKey(d => d.TipoUnidadId)
                .HasConstraintName("unidad_medica_tipo_unidad_id_fkey");
        });

        modelBuilder.Entity<UnidadMedicaAlias>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("unidad_medica_alias_pkey");

            entity.ToTable("unidad_medica_alias");

            entity.HasIndex(e => e.Id, "idx_uma_id");

            entity.HasIndex(e => e.UnidadMedicaId, "idx_uma_unidad_medica");

            entity.HasIndex(e => e.UnidadMedicaId, "unidad_medica_alias_unidad_medica_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.AliasDash)
                .HasMaxLength(50)
                .HasColumnName("alias_dash");
            entity.Property(e => e.AliasSas)
                .HasMaxLength(150)
                .HasColumnName("alias_sas");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("creado_en");
            entity.Property(e => e.UnidadMedicaId).HasColumnName("unidad_medica_id");

            entity.HasOne(d => d.UnidadMedica).WithOne(p => p.UnidadMedicaAlias)
                .HasForeignKey<UnidadMedicaAlias>(d => d.UnidadMedicaId)
                .HasConstraintName("unidad_medica_alias_unidad_medica_id_fkey");
        });

        modelBuilder.Entity<UnidadMedicaKit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("unidad_medica_kit_pkey");

            entity.ToTable("unidad_medica_kit");

            entity.HasIndex(e => new { e.UnidadMedicaId, e.KitId }, "uq_unidad_medica_kit").IsUnique();

            entity.HasIndex(e => new { e.UnidadMedicaId, e.KitId }, "uq_unidad_medica_kit_pair").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.Fuente).HasColumnName("fuente");
            entity.Property(e => e.KitId).HasColumnName("kit_id");
            entity.Property(e => e.UnidadMedicaId).HasColumnName("unidad_medica_id");

            entity.HasOne(d => d.Kit).WithMany(p => p.UnidadMedicaKits)
                .HasForeignKey(d => d.KitId)
                .HasConstraintName("unidad_medica_kit_kit_id_fkey");

            entity.HasOne(d => d.UnidadMedica).WithMany(p => p.UnidadMedicaKits)
                .HasForeignKey(d => d.UnidadMedicaId)
                .HasConstraintName("unidad_medica_kit_unidad_medica_id_fkey");
        });

        modelBuilder.Entity<VCpmBc>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_cpm_bc")
                .HasAnnotation("Npgsql:StorageParameter:security_invoker", "on");

            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(20)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.Cluesimb)
                .HasMaxLength(20)
                .HasColumnName("cluesimb");
            entity.Property(e => e.Cluessa)
                .HasMaxLength(20)
                .HasColumnName("cluessa");
            entity.Property(e => e.Cpm).HasColumnName("cpm");
            entity.Property(e => e.CreadoEn).HasColumnName("creado_en");
            entity.Property(e => e.Fuente).HasColumnName("fuente");
            entity.Property(e => e.FuenteRaw)
                .HasMaxLength(255)
                .HasColumnName("fuente_raw");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UnidadMedicaId).HasColumnName("unidad_medica_id");
        });

        modelBuilder.Entity<VCpmDiferencia>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_cpm_diferencias");

            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(20)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.Cluesimb)
                .HasMaxLength(20)
                .HasColumnName("cluesimb");
            entity.Property(e => e.CpmCdmx).HasColumnName("cpm_cdmx");
            entity.Property(e => e.CpmPropuesto).HasColumnName("cpm_propuesto");
            entity.Property(e => e.Diferencia).HasColumnName("diferencia");
            entity.Property(e => e.NombreDeUnidad)
                .HasMaxLength(255)
                .HasColumnName("nombre_de_unidad");
            entity.Property(e => e.Observacion).HasColumnName("observacion");
        });

        modelBuilder.Entity<VCpmReal>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_cpm_real")
                .HasAnnotation("Npgsql:StorageParameter:security_invoker", "on");

            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(20)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.Cluesimb)
                .HasMaxLength(20)
                .HasColumnName("cluesimb");
            entity.Property(e => e.Cluessa)
                .HasMaxLength(20)
                .HasColumnName("cluessa");
            entity.Property(e => e.Cpm).HasColumnName("cpm");
            entity.Property(e => e.CreadoEn).HasColumnName("creado_en");
            entity.Property(e => e.Fuente).HasColumnName("fuente");
            entity.Property(e => e.FuenteRaw).HasColumnName("fuente_raw");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UnidadMedicaId).HasColumnName("unidad_medica_id");
        });

        modelBuilder.Entity<VExistenciasConsolidada>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_existencias_consolidadas");

            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(50)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.Cluesimb)
                .HasMaxLength(20)
                .HasColumnName("cluesimb");
            entity.Property(e => e.Cluessa)
                .HasMaxLength(20)
                .HasColumnName("cluessa");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Existencia).HasColumnName("existencia");
            entity.Property(e => e.NombreDeUnidad)
                .HasMaxLength(255)
                .HasColumnName("nombre_de_unidad");
            entity.Property(e => e.NombreMunicipio)
                .HasMaxLength(100)
                .HasColumnName("nombre_municipio");
        });

        modelBuilder.Entity<VMovimientosAUnidadesDesdeAbasto>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_movimientos_a_unidades_desde_abasto")
                .HasAnnotation("Npgsql:StorageParameter:security_invoker", "on");

            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(50)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.CluesDestino)
                .HasMaxLength(20)
                .HasColumnName("clues_destino");
            entity.Property(e => e.FechaCaducidad).HasColumnName("fecha_caducidad");
            entity.Property(e => e.FechaMovimiento).HasColumnName("fecha_movimiento");
            entity.Property(e => e.Lote)
                .HasMaxLength(150)
                .HasColumnName("lote");
            entity.Property(e => e.Programa).HasColumnName("programa");
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(50)
                .HasColumnName("tipo_movimiento");
            entity.Property(e => e.Total).HasColumnName("total");
            entity.Property(e => e.UnidadDestinoTexto)
                .HasMaxLength(255)
                .HasColumnName("unidad_destino_texto");
            entity.Property(e => e.UnidadOrigenTexto).HasColumnName("unidad_origen_texto");
        });

        modelBuilder.Entity<VOncoAbastoCpm>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_onco_abasto_cpm");

            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(50)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.Cluesimb)
                .HasMaxLength(50)
                .HasColumnName("cluesimb");
            entity.Property(e => e.Cpm).HasColumnName("cpm");
            entity.Property(e => e.CpmX3).HasColumnName("cpm_x_3");
            entity.Property(e => e.CpmsEq).HasColumnName("cpms_eq");
            entity.Property(e => e.EstadoAbasto).HasColumnName("estado_abasto");
            entity.Property(e => e.Existencias).HasColumnName("existencias");
            entity.Property(e => e.NombreDeUnidad)
                .HasMaxLength(255)
                .HasColumnName("nombre_de_unidad");
        });

        modelBuilder.Entity<VReporteBalanceoJurisdiccional>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_reporte_balanceo_jurisdiccional");

            entity.Property(e => e.CantidadApartada).HasColumnName("cantidad_apartada");
            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(30)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.CpmJurisdiccional).HasColumnName("cpm_jurisdiccional");
            entity.Property(e => e.CubreCpmJurisdiccional).HasColumnName("cubre_cpm_jurisdiccional");
            entity.Property(e => e.DeltaVsCpm).HasColumnName("delta_vs_cpm");
            entity.Property(e => e.EjecucionId).HasColumnName("ejecucion_id");
            entity.Property(e => e.ExcedenteFinal).HasColumnName("excedente_final");
            entity.Property(e => e.ExistenciaBalanceableInicial).HasColumnName("existencia_balanceable_inicial");
            entity.Property(e => e.ExistenciaOriginalAlmacen).HasColumnName("existencia_original_almacen");
            entity.Property(e => e.Jurisdiccion)
                .HasMaxLength(100)
                .HasColumnName("jurisdiccion");
            entity.Property(e => e.RecibidoDeOtros).HasColumnName("recibido_de_otros");
            entity.Property(e => e.TransferidoAOtros).HasColumnName("transferido_a_otros");
        });

        modelBuilder.Entity<VUnidadCpm>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_unidad_cpm")
                .HasAnnotation("Npgsql:StorageParameter:security_barrier", "on")
                .HasAnnotation("Npgsql:StorageParameter:security_invoker", "on");

            entity.Property(e => e.ClaveCnis)
                .HasMaxLength(20)
                .HasColumnName("clave_cnis");
            entity.Property(e => e.Cluesimb)
                .HasMaxLength(20)
                .HasColumnName("cluesimb");
            entity.Property(e => e.Cpm).HasColumnName("cpm");
            entity.Property(e => e.NombreUnidad)
                .HasMaxLength(255)
                .HasColumnName("nombre_unidad");
            entity.Property(e => e.UnidadMedicaId).HasColumnName("unidad_medica_id");
        });

        modelBuilder.Entity<VUnidadKitClavesExpectedVsCpm>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_unidad_kit_claves_expected_vs_cpm")
                .HasAnnotation("Npgsql:StorageParameter:security_barrier", "on")
                .HasAnnotation("Npgsql:StorageParameter:security_invoker", "on");

            entity.Property(e => e.ClaveCnis).HasColumnName("clave_cnis");
            entity.Property(e => e.Cluesimb)
                .HasMaxLength(20)
                .HasColumnName("cluesimb");
            entity.Property(e => e.Cluessa)
                .HasMaxLength(20)
                .HasColumnName("cluessa");
            entity.Property(e => e.Cpm).HasColumnName("cpm");
            entity.Property(e => e.EnCpm).HasColumnName("en_cpm");
            entity.Property(e => e.Fuentes).HasColumnName("fuentes");
            entity.Property(e => e.KitCodigo).HasColumnName("kit_codigo");
            entity.Property(e => e.KitCodigos).HasColumnName("kit_codigos");
            entity.Property(e => e.KitCodigosTxt).HasColumnName("kit_codigos_txt");
            entity.Property(e => e.KitIds).HasColumnName("kit_ids");
            entity.Property(e => e.NombreTipologia).HasColumnName("nombre_tipologia");
            entity.Property(e => e.NombreUnidad)
                .HasMaxLength(255)
                .HasColumnName("nombre_unidad");
            entity.Property(e => e.UnidadMedicaId).HasColumnName("unidad_medica_id");
        });

        modelBuilder.Entity<VUnidadKitClavesExpectedVsCpmV2>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_unidad_kit_claves_expected_vs_cpm_v2")
                .HasAnnotation("Npgsql:StorageParameter:security_barrier", "on")
                .HasAnnotation("Npgsql:StorageParameter:security_invoker", "on");

            entity.Property(e => e.ClaveCnis).HasColumnName("clave_cnis");
            entity.Property(e => e.Cluesimb)
                .HasMaxLength(20)
                .HasColumnName("cluesimb");
            entity.Property(e => e.Cluessa)
                .HasMaxLength(20)
                .HasColumnName("cluessa");
            entity.Property(e => e.Cpm).HasColumnName("cpm");
            entity.Property(e => e.EnCpm).HasColumnName("en_cpm");
            entity.Property(e => e.Fuentes).HasColumnName("fuentes");
            entity.Property(e => e.KitCodigo).HasColumnName("kit_codigo");
            entity.Property(e => e.KitCodigos).HasColumnName("kit_codigos");
            entity.Property(e => e.KitCodigosTxt).HasColumnName("kit_codigos_txt");
            entity.Property(e => e.KitIds).HasColumnName("kit_ids");
            entity.Property(e => e.NombreTipologia).HasColumnName("nombre_tipologia");
            entity.Property(e => e.NombreUnidad)
                .HasMaxLength(255)
                .HasColumnName("nombre_unidad");
            entity.Property(e => e.UnidadMedicaId).HasColumnName("unidad_medica_id");
        });

        modelBuilder.Entity<VUnidadMedicaDetalle>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_unidad_medica_detalle")
                .HasAnnotation("Npgsql:StorageParameter:security_barrier", "on")
                .HasAnnotation("Npgsql:StorageParameter:security_invoker", "on");

            entity.Property(e => e.AliasSas)
                .HasMaxLength(150)
                .HasColumnName("alias_sas");
            entity.Property(e => e.Cluesimb)
                .HasMaxLength(20)
                .HasColumnName("cluesimb");
            entity.Property(e => e.Cluessa)
                .HasMaxLength(20)
                .HasColumnName("cluessa");
            entity.Property(e => e.Direccion).HasColumnName("direccion");
            entity.Property(e => e.EsSegundoNivel).HasColumnName("es_segundo_nivel");
            entity.Property(e => e.EstratoUnidad)
                .HasMaxLength(10)
                .HasColumnName("estrato_unidad");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Latitud)
                .HasPrecision(10, 6)
                .HasColumnName("latitud");
            entity.Property(e => e.Longitud)
                .HasPrecision(10, 6)
                .HasColumnName("longitud");
            entity.Property(e => e.NivelAtencion)
                .HasMaxLength(30)
                .HasColumnName("nivel_atencion");
            entity.Property(e => e.NombreDeUnidad)
                .HasMaxLength(255)
                .HasColumnName("nombre_de_unidad");
            entity.Property(e => e.NombreLocalidad)
                .HasMaxLength(150)
                .HasColumnName("nombre_localidad");
            entity.Property(e => e.NombreMunicipio)
                .HasMaxLength(100)
                .HasColumnName("nombre_municipio");
            entity.Property(e => e.NombreTipologia).HasColumnName("nombre_tipologia");
            entity.Property(e => e.TipoUnidad)
                .HasMaxLength(100)
                .HasColumnName("tipo_unidad");
        });

        modelBuilder.Entity<VUnidadMedicaKitClafe>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_unidad_medica_kit_claves")
                .HasAnnotation("Npgsql:StorageParameter:security_barrier", "on")
                .HasAnnotation("Npgsql:StorageParameter:security_invoker", "on");

            entity.Property(e => e.ClaveCnis).HasColumnName("clave_cnis");
            entity.Property(e => e.Cluesimb)
                .HasMaxLength(20)
                .HasColumnName("cluesimb");
            entity.Property(e => e.KitCodigo).HasColumnName("kit_codigo");
            entity.Property(e => e.NombreTipologia).HasColumnName("nombre_tipologia");
            entity.Property(e => e.UnidadMedicaId).HasColumnName("unidad_medica_id");
        });
        modelBuilder.HasSequence("jobid_seq", "cron");
        modelBuilder.HasSequence("runid_seq", "cron");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
