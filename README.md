# imssb-bc-net-backend

Demo funcional de Web API en .NET 8 para autenticacion con JWT, refresh tokens one-time-use, PostgreSQL 17 y una arquitectura por capas ligera.

## Arquitectura

- `Domain`: entidades `User` y `UserRefreshToken`.
- `Application`: CQRS con MediatR, DTOs, contratos, handlers y validaciones FluentValidation.
- `Infrastructure`: EF Core, PostgreSQL, repositorios, BCrypt, JWT, middleware global de errores.
- `WebAPI`: Controllers tradicionales, Swagger, CORS, health check y configuracion HTTP.

## Ejecutar localmente

1. Levanta PostgreSQL:

```bash
docker compose up -d
```

Si antes levantaste el proyecto con PostgreSQL 16, Docker puede conservar un volumen incompatible. Este compose usa `postgres17_data` para iniciar datos limpios en PostgreSQL 17 sin borrar el volumen anterior.

Si moviste la BD local y quieres empezar desde cero, puedes borrar solo el volumen local de este demo:

```bash
docker compose down -v
docker compose up -d
```

2. Protege secretos locales con User Secrets:

```bash
dotnet user-secrets init --project src/WebAPI/WebAPI.csproj
dotnet user-secrets set "JwtSettings:SecretKey" "change-this-local-secret-with-at-least-32-chars" --project src/WebAPI/WebAPI.csproj
```

3. Ejecuta la API:

```bash
dotnet run --project src/WebAPI/WebAPI.csproj
```

La API escucha en `http://localhost:8080` porque `Program.cs` lee `PORT` y usa `8080` como fallback.

Usuario demo sembrado:

```text
email: demo@example.com
password: Password123!
```

## Endpoints

```bash
curl http://localhost:8080/api/ping
```

```bash
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"demo@example.com\",\"password\":\"Password123!\"}"
```

```bash
curl http://localhost:8080/api/user/me \
  -H "Authorization: Bearer ACCESS_TOKEN"
```

```bash
curl -X POST http://localhost:8080/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d "{\"refreshToken\":\"REFRESH_TOKEN\"}"
```

```bash
curl -X POST http://localhost:8080/api/auth/logout \
  -H "Authorization: Bearer ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d "{\"refreshToken\":\"REFRESH_TOKEN\"}"
```

## Render

Variables requeridas:

```text
DATABASE_URL=postgresql://user:pass@host:5432/db
JwtSettings__SecretKey=min-32-characters-strong-secret
JwtSettings__Issuer=imssb-bc-net-backend
JwtSettings__Audience=imssb-bc-net-frontend
ASPNETCORE_ENVIRONMENT=Production
CORS_ALLOWED_ORIGINS=https://tu-frontend.com
```

Health check: `/health`.

## Pruebas

```bash
dotnet test
```

Incluye pruebas de ejemplo para credenciales invalidas y refresh token valido.

## Migraciones EF Core

El demo usa `EnsureCreatedAsync` para crear el esquema rapidamente al arrancar. Para un flujo mas formal con migraciones:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project src/Infrastructure --startup-project src/WebAPI
dotnet ef database update --project src/Infrastructure --startup-project src/WebAPI
```

## Extensiones VS Code sugeridas

- C# Dev Kit
- C# Extensions
- NuGet Gallery
- REST Client
- Docker
- GitLens
- EditorConfig for VS Code

## Monitoreo

Para empezar en Render, Serilog a consola es suficiente porque Render captura stdout. Cuando quieras trazabilidad de errores real, la opcion mas practica es Sentry; OpenTelemetry + Grafana Cloud es mejor si despues necesitas trazas y metricas multi-servicio.
