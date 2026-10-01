using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Tests;

public sealed class SolicitudesAuthorizationTests
{
    [Theory]
    [InlineData("IB_ONCO", true)]
    [InlineData("SOLICITUDES_ABASTO", true)]
    [InlineData("ADMIN_TIC", true)]
    [InlineData("COORDINACION", true)]
    [InlineData("ABASTO", true)]
    [InlineData("UNIDAD_MEDICA", true)]
    [InlineData("ENFERMERIA", true)]
    [InlineData("OTRO", false)]
    public async Task SolicitudesAccess_AuthorizesConfiguredRoles(string role, bool expected)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.Role, role)], "Test"));
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorization(options => options.AddPolicy("SolicitudesAccess", policy => policy
            .RequireRole("IB_ONCO", "SOLICITUDES_ABASTO", "ADMIN_TIC", "COORDINACION", "ABASTO", "UNIDAD_MEDICA", "ENFERMERIA")));
        using var provider = services.BuildServiceProvider();

        var authorization = provider.GetRequiredService<IAuthorizationService>();

        Assert.Equal(expected, (await authorization.AuthorizeAsync(user, null, "SolicitudesAccess")).Succeeded);
    }
}
