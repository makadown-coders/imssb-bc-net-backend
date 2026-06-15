using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await EnsureSchemaAsync(dbContext, cancellationToken);

        if (await dbContext.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        dbContext.Users.Add(new User
        {
            Email = "demo@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!", 12),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task EnsureSchemaAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (await TableExistsAsync(dbContext, "Users", cancellationToken)
            && await TableExistsAsync(dbContext, "UserRefreshTokens", cancellationToken))
        {
            return;
        }

        await dbContext.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS "Users" (
                "Id" uuid NOT NULL,
                "Email" character varying(256) NOT NULL,
                "PasswordHash" character varying(256) NOT NULL,
                "CreatedAt" timestamp with time zone NOT NULL,
                "IsActive" boolean NOT NULL,
                CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
            );

            CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Email" ON "Users" ("Email");

            CREATE TABLE IF NOT EXISTS "UserRefreshTokens" (
                "Id" uuid NOT NULL,
                "UserId" uuid NOT NULL,
                "Token" character varying(128) NOT NULL,
                "ExpiresUtc" timestamp with time zone NOT NULL,
                "IsRevoked" boolean NOT NULL,
                "CreatedDate" timestamp with time zone NOT NULL,
                "RevokedAtUtc" timestamp with time zone NULL,
                CONSTRAINT "PK_UserRefreshTokens" PRIMARY KEY ("Id"),
                CONSTRAINT "FK_UserRefreshTokens_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
            );

            CREATE UNIQUE INDEX IF NOT EXISTS "IX_UserRefreshTokens_Token" ON "UserRefreshTokens" ("Token");
            CREATE INDEX IF NOT EXISTS "IX_UserRefreshTokens_UserId" ON "UserRefreshTokens" ("UserId");
            """, cancellationToken);
    }

    private static async Task<bool> TableExistsAsync(
        AppDbContext dbContext,
        string tableName,
        CancellationToken cancellationToken)
    {
        var connection = dbContext.Database.GetDbConnection();
        var shouldClose = connection.State == System.Data.ConnectionState.Closed;

        if (shouldClose)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = 'public'
                      AND table_name = @tableName
                );
                """;

            var parameter = command.CreateParameter();
            parameter.ParameterName = "tableName";
            parameter.Value = tableName;
            command.Parameters.Add(parameter);

            var result = await command.ExecuteScalarAsync(cancellationToken);
            return result is true;
        }
        finally
        {
            if (shouldClose)
            {
                await connection.CloseAsync();
            }
        }
    }
}
