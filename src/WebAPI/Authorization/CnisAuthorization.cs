using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Authorization;

public static class CnisAuthorization
{
    public const string ReadPolicy = "CnisReadAccess";

    public static void AddCnisReadPolicy(this AuthorizationOptions options) =>
        options.AddPolicy(ReadPolicy, policy => policy
            .RequireAuthenticatedUser()
            .RequireRole("ADMIN_TIC", "IB_ONCO", "UNIDAD_MEDICA", "ENFERMERIA"));
}
