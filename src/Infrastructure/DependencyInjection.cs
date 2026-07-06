using Application.Interfaces;
using Application.Features.Solicitudes.Catalogos;
using Application.Features.Solicitudes;
using Infrastructure.Configuration;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Solicitudes;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.Solicitudes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(nameof(JwtSettings)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var connectionString = DatabaseUrlParser.BuildConnectionString(
            Environment.GetEnvironmentVariable("DATABASE_URL"),
            configuration.GetConnectionString("DefaultConnection"));

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        // Contexto separado para no mezclar el modelo operativo con el agregado de identidad.
        services.AddDbContext<SolicitudesDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserRefreshTokenRepository, UserRefreshTokenRepository>();
        services.AddScoped<IOperationalCatalogService, OperationalCatalogService>();
        services.AddScoped<ISolicitudesCaptureService, SolicitudesCaptureService>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenService, JwtService>();

        return services;
    }
}
