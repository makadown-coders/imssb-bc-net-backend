using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<TipoUnidad> TiposUnidad => Set<TipoUnidad>();
    public DbSet<Municipio> Municipios => Set<Municipio>();
    public DbSet<Localidad> Localidades => Set<Localidad>();
    public DbSet<UnidadMedica> UnidadesMedicas => Set<UnidadMedica>();
    public DbSet<Tipologia> Tipologias => Set<Tipologia>();
    public DbSet<TipologiaUnidad> TipologiasUnidad => Set<TipologiaUnidad>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(user => user.Id);
            entity.HasIndex(user => user.Email).IsUnique();
            entity.Property(user => user.Email).HasMaxLength(256).IsRequired();
            entity.Property(user => user.PasswordHash).HasMaxLength(256).IsRequired();
            entity.Property(user => user.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<UserRefreshToken>(entity =>
        {
            entity.ToTable("UserRefreshTokens");
            entity.HasKey(token => token.Id);
            entity.HasIndex(token => token.Token).IsUnique();
            entity.Property(token => token.Token).HasMaxLength(128).IsRequired();
            entity.Property(token => token.ExpiresUtc).IsRequired();
            entity.Property(token => token.CreatedDate).IsRequired();
            entity.HasOne(token => token.User)
                .WithMany(user => user.RefreshTokens)
                .HasForeignKey(token => token.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(role => role.Code);
            entity.Property(role => role.Code).HasColumnName("code").HasMaxLength(80).IsRequired();
            entity.Property(role => role.Descripcion).HasColumnName("descripcion").HasMaxLength(200).IsRequired();
            entity.Property(role => role.IsActive).HasColumnName("is_active").HasDefaultValue(true).IsRequired();
            entity.Property(role => role.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()").IsRequired();
            entity.Property(role => role.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("user_roles");
            entity.HasKey(userRole => new { userRole.UserId, userRole.RoleCode });
            entity.Property(userRole => userRole.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(userRole => userRole.RoleCode).HasColumnName("role_code").HasMaxLength(80).IsRequired();
            entity.Property(userRole => userRole.AssignedAt).HasColumnName("assigned_at").HasDefaultValueSql("now()").IsRequired();
            entity.Property(userRole => userRole.AssignedByUserId).HasColumnName("assigned_by_user_id");
            entity.Property(userRole => userRole.RevokedAt).HasColumnName("revoked_at");
            entity.Property(userRole => userRole.IsActive).HasColumnName("is_active").HasDefaultValue(true).IsRequired();
            entity.HasOne(userRole => userRole.User)
                .WithMany(user => user.UserRoles)
                .HasForeignKey(userRole => userRole.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(userRole => userRole.Role)
                .WithMany(role => role.UserRoles)
                .HasForeignKey(userRole => userRole.RoleCode)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(userRole => userRole.AssignedByUser)
                .WithMany()
                .HasForeignKey(userRole => userRole.AssignedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<TipoUnidad>(entity =>
        {
            entity.ToTable("tipo_unidad");
            entity.HasKey(tipo => tipo.Id);
            entity.HasIndex(tipo => tipo.NombreTipo).IsUnique();
            entity.Property(tipo => tipo.Id).HasColumnName("id");
            entity.Property(tipo => tipo.NombreTipo).HasColumnName("nombre_tipo").HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Municipio>(entity =>
        {
            entity.ToTable("municipio");
            entity.HasKey(municipio => municipio.Id);
            entity.HasIndex(municipio => municipio.NombreMunicipio).IsUnique();
            entity.Property(municipio => municipio.Id).HasColumnName("id");
            entity.Property(municipio => municipio.NombreMunicipio).HasColumnName("nombre_municipio").HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Localidad>(entity =>
        {
            entity.ToTable("localidad");
            entity.HasKey(localidad => localidad.Id);
            entity.HasIndex(localidad => new { localidad.NombreLocalidad, localidad.MunicipioId }).IsUnique();
            entity.Property(localidad => localidad.Id).HasColumnName("id");
            entity.Property(localidad => localidad.NombreLocalidad).HasColumnName("nombre_localidad").HasMaxLength(150).IsRequired();
            entity.Property(localidad => localidad.MunicipioId).HasColumnName("municipio_id");
            entity.HasOne(localidad => localidad.Municipio)
                .WithMany(municipio => municipio.Localidades)
                .HasForeignKey(localidad => localidad.MunicipioId);
        });

        modelBuilder.Entity<UnidadMedica>(entity =>
        {
            entity.ToTable("unidad_medica");
            entity.HasKey(unidad => unidad.Id);
            entity.Property(unidad => unidad.Id).HasColumnName("id");
            entity.Property(unidad => unidad.Cluessa).HasColumnName("cluessa").HasMaxLength(20);
            entity.Property(unidad => unidad.Cluesimb).HasColumnName("cluesimb").HasMaxLength(20);
            entity.Property(unidad => unidad.Nombre).HasColumnName("nombre").HasMaxLength(255).IsRequired();
            entity.Property(unidad => unidad.Direccion).HasColumnName("direccion");
            entity.Property(unidad => unidad.Latitud).HasColumnName("latitud").HasPrecision(10, 6);
            entity.Property(unidad => unidad.Longitud).HasColumnName("longitud").HasPrecision(10, 6);
            entity.Property(unidad => unidad.EstratoUnidad).HasColumnName("estrato_unidad").HasMaxLength(10);
            entity.Property(unidad => unidad.NivelAtencion).HasColumnName("nivel_atencion").HasMaxLength(30);
            entity.Property(unidad => unidad.TipoUnidadId).HasColumnName("tipo_unidad_id");
            entity.Property(unidad => unidad.LocalidadId).HasColumnName("localidad_id");
            entity.Property(unidad => unidad.Activo).HasColumnName("activo").HasDefaultValue(true).IsRequired();
            entity.HasOne(unidad => unidad.TipoUnidad)
                .WithMany(tipo => tipo.UnidadesMedicas)
                .HasForeignKey(unidad => unidad.TipoUnidadId);
            entity.HasOne(unidad => unidad.Localidad)
                .WithMany(localidad => localidad.UnidadesMedicas)
                .HasForeignKey(unidad => unidad.LocalidadId);
        });

        modelBuilder.Entity<Tipologia>(entity =>
        {
            entity.ToTable("tipologia");
            entity.HasKey(tipologia => tipologia.Id);
            entity.Property(tipologia => tipologia.Id).HasColumnName("id");
            entity.Property(tipologia => tipologia.Nombre).HasColumnName("nombre").IsRequired();
            entity.Property(tipologia => tipologia.EsSegundoNivel).HasColumnName("es_segundo_nivel").HasDefaultValue(false);
        });

        modelBuilder.Entity<TipologiaUnidad>(entity =>
        {
            entity.ToTable("tipologia_unidad");
            entity.HasKey(tipologiaUnidad => tipologiaUnidad.Id);
            entity.HasIndex(tipologiaUnidad => tipologiaUnidad.UnidadMedicaId).IsUnique();
            entity.Property(tipologiaUnidad => tipologiaUnidad.Id).HasColumnName("id");
            entity.Property(tipologiaUnidad => tipologiaUnidad.UnidadMedicaId).HasColumnName("unidad_medica_id").IsRequired();
            entity.Property(tipologiaUnidad => tipologiaUnidad.TipologiaId).HasColumnName("tipologia_id").IsRequired();
            entity.Property(tipologiaUnidad => tipologiaUnidad.Fuente).HasColumnName("fuente");
            entity.Property(tipologiaUnidad => tipologiaUnidad.CreadoEn).HasColumnName("creado_en").HasDefaultValueSql("now()");
            entity.HasOne(tipologiaUnidad => tipologiaUnidad.UnidadMedica)
                .WithOne(unidad => unidad.TipologiaUnidad)
                .HasForeignKey<TipologiaUnidad>(tipologiaUnidad => tipologiaUnidad.UnidadMedicaId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(tipologiaUnidad => tipologiaUnidad.Tipologia)
                .WithMany(tipologia => tipologia.TipologiasUnidad)
                .HasForeignKey(tipologiaUnidad => tipologiaUnidad.TipologiaId);
        });
    }
}
