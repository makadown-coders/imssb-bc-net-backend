using Npgsql;

namespace Infrastructure.Persistence;

public static class DatabaseUrlParser
{
    public static string BuildConnectionString(string? databaseUrl, string? fallback)
    {
        if (string.IsNullOrWhiteSpace(databaseUrl))
        {
            return fallback ?? throw new InvalidOperationException(
                "DATABASE_URL or ConnectionStrings:DefaultConnection is required.");
        }

        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':', 2);

        return new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty,
            SslMode = SslMode.Require
        }.ConnectionString;
    }
}
