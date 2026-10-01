using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Authorization;
using WebAPI.Controllers;

namespace Application.Tests;

public sealed class CnisAuthorizationTests
{
    [Theory]
    [InlineData("ADMIN_TIC", true)]
    [InlineData("IB_ONCO", true)]
    [InlineData("UNIDAD_MEDICA", true)]
    [InlineData("ENFERMERIA", true)]
    [InlineData("ABASTO", false)]
    [InlineData("COORDINACION", false)]
    [InlineData("SOLICITUDES_ABASTO", false)]
    [InlineData("OTRO", false)]
    [InlineData(null, false)]
    public async Task ReadPolicy_AuthorizesOnlyAgreedRoles(string? role, bool expected)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            role is null ? [] : new[] { new Claim(ClaimTypes.Role, role) }, "Test"));

        Assert.Equal(expected, await IsAuthorized(user));
    }

    [Fact]
    public async Task ReadPolicy_RejectsUnauthenticatedIdentityEvenWithRole()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Role, "ADMIN_TIC") }));

        Assert.False(await IsAuthorized(user));
    }

    [Fact]
    public async Task ReadPolicy_AcceptsAllowedRoleAmongMultipleRoles()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, "COORDINACION"),
            new Claim(ClaimTypes.Role, "ENFERMERIA")
        }, "Test"));

        Assert.True(await IsAuthorized(user));
    }

    [Theory]
    [InlineData(nameof(CnisGruposTerapeuticosController.GetGrupos))]
    [InlineData(nameof(CnisGruposTerapeuticosController.GetAsociaciones))]
    [InlineData(nameof(CnisGruposTerapeuticosController.GetArticulos))]
    public void ReadEndpoints_RequireCnisPolicy(string action)
    {
        var controller = typeof(CnisGruposTerapeuticosController);
        var method = controller.GetMethod(action)!;
        Assert.Equal(CnisAuthorization.ReadPolicy,
            Assert.Single(controller.GetCustomAttributes<AuthorizeAttribute>()).Policy);
        Assert.Empty(controller.GetCustomAttributes<AllowAnonymousAttribute>());
        Assert.Empty(method.GetCustomAttributes<AllowAnonymousAttribute>());
        Assert.NotNull(method.GetCustomAttribute<HttpGetAttribute>());
    }

    private static async Task<bool> IsAuthorized(ClaimsPrincipal user)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorization(options => options.AddCnisReadPolicy());
        using var provider = services.BuildServiceProvider();
        var authorization = provider.GetRequiredService<IAuthorizationService>();
        return (await authorization.AuthorizeAsync(user, null, CnisAuthorization.ReadPolicy)).Succeeded;
    }
}
