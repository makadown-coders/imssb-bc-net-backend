# IMSS Bienestar BC — Backend

API REST en .NET 8 para autenticación, administración de Personas, Usuarios, roles y catálogos institucionales. Utiliza JWT, refresh tokens de un solo uso, BCrypt, PostgreSQL 17 y una arquitectura por capas ligera.

## Arquitectura

- `Domain`: entidades del negocio (`User`, `Persona`, `Role`, `UserRole`, catálogos y refresh tokens).
- `Application`: casos de uso con MediatR, DTOs, interfaces, handlers y validaciones con FluentValidation.
- `Infrastructure`: Entity Framework Core, PostgreSQL, repositorios, BCrypt, JWT y middleware global de errores.
- `WebAPI`: controladores, contratos HTTP, autorización, Swagger, CORS, rate limiting y health check.
- `tests`: pruebas unitarias de autenticación y contraseñas.

## Reglas principales del sistema

### Personas y Usuarios

Una **Persona** representa el expediente humano y organizacional. Un **Usuario** representa una cuenta de acceso.

- Las cuentas normales se aprovisionan desde una Persona activa con correo principal.
- Una Persona solo puede estar vinculada con un Usuario y viceversa.
- No existe un endpoint público para crear Usuarios de forma independiente.
- `demo@example.com` es una cuenta técnica de desarrollo y puede no tener Persona vinculada.

### Roles

- Un Usuario puede tener varios roles activos.
- La asignación conserva quién asignó el rol y cuándo lo hizo.
- Revocar un rol no elimina el registro: establece `IsActive = false` y registra `RevokedAt`.
- Asignar o revocar roles invalida los refresh tokens del Usuario afectado.
- `ADMIN_TIC` es un rol protegido: no se muestra como rol asignable y no puede asignarse ni revocarse mediante los módulos de Personas o Usuarios.
- Los roles forman parte del JWT. Un access token ya emitido conserva sus claims hasta expirar; la revocación de refresh tokens impide renovar esa sesión.

### Contraseñas

- Un Usuario autenticado cambia su propia contraseña proporcionando la contraseña actual.
- Un `ADMIN_TIC` puede restablecer la contraseña de otro Usuario sin conocer la anterior.
- Las contraseñas nuevas deben tener entre 12 y 128 caracteres.
- Todo cambio o restablecimiento revoca los refresh tokens del Usuario.
- Las contraseñas se almacenan mediante BCrypt con factor de trabajo 12.

## Requisitos

- .NET SDK 8
- Docker Desktop o PostgreSQL 17

## Ejecutar localmente

1. Levanta PostgreSQL:

```bash
docker compose up -d
```

Este compose utiliza el volumen `postgres17_data`. Para reiniciar únicamente la base local del proyecto:

```bash
docker compose down -v
docker compose up -d
```

2. Configura el secreto JWT local:

```bash
dotnet user-secrets init --project src/WebAPI/WebAPI.csproj
dotnet user-secrets set "JwtSettings:SecretKey" "change-this-local-secret-with-at-least-32-chars" --project src/WebAPI/WebAPI.csproj
```

3. Ejecuta la API:

```bash
dotnet run --project src/WebAPI/WebAPI.csproj
```

La API escucha en `http://localhost:8080`. Al modificar controladores o handlers, reinicia el proceso para cargar el nuevo ensamblado.

### Usuario de desarrollo

```text
email: demo@example.com
password: P**********!
rol: ADMIN_TIC
```

## Autenticación

### Iniciar sesión

```bash
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"demo@example.com","password":"P**********!"}'
```

La respuesta contiene `accessToken`, `refreshToken` y sus fechas de expiración.

### Consultar el Usuario actual

```bash
curl http://localhost:8080/api/user/me \
  -H "Authorization: Bearer ACCESS_TOKEN"
```

### Renovar tokens

Cada refresh token es de un solo uso. Al renovarlo, el anterior queda revocado.

```bash
curl -X POST http://localhost:8080/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{"refreshToken":"REFRESH_TOKEN"}'
```

### Cerrar sesión

```bash
curl -X POST http://localhost:8080/api/auth/logout \
  -H "Authorization: Bearer ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"refreshToken":"REFRESH_TOKEN"}'
```

## Contraseñas

### Cambio de contraseña propia

```bash
curl -X PUT http://localhost:8080/api/user/me/password \
  -H "Authorization: Bearer ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"currentPassword":"P**********!","newPassword":"NuevaP**********!"}'
```

### Restablecimiento administrativo

Requiere `ADMIN_TIC` y debe dirigirse a otro Usuario:

```bash
curl -X PUT http://localhost:8080/api/users/USER_ID/password \
  -H "Authorization: Bearer ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"newPassword":"TemporalP**********!"}'
```

También existe la ruta compatible `PUT /api/user/{userId}/password`.

## Endpoints principales

Todos los endpoints administrativos requieren un JWT con rol `ADMIN_TIC`.

### Personas

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/personas` | Lista y filtra Personas. |
| `GET` | `/api/personas/{id}` | Obtiene una Persona. |
| `POST` | `/api/personas` | Crea una Persona. |
| `PUT` | `/api/personas/{id}` | Actualiza una Persona. |
| `DELETE` | `/api/personas/{id}` | Da de baja una Persona. |
| `POST` | `/api/personas/{id}/usuario` | Crea una cuenta y la vincula con la Persona. |
| `PUT` | `/api/personas/{id}/usuario` | Vincula un Usuario existente. |
| `DELETE` | `/api/personas/{id}/usuario` | Desvincula la cuenta. |

Filtros de listado: `q`, `activo`, `unidadMedicaId`, `page` y `pageSize`.

### Usuarios y roles

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/users` | Lista Usuarios con Persona, Unidad y roles activos. |
| `GET` | `/api/users/{userId}/roles` | Consulta roles activos del Usuario. |
| `POST` | `/api/users/{userId}/roles/{roleCode}` | Asigna o reactiva un rol. |
| `DELETE` | `/api/users/{userId}/roles/{roleCode}` | Revoca un rol sin borrar el historial. |
| `PUT` | `/api/users/{userId}/password` | Restablece la contraseña. |
| `GET` | `/api/roles` | Lista roles activos asignables; excluye `ADMIN_TIC`. |

Filtros de Usuarios: `q`, `isActive`, `unidadId`, `roleCode`, `page` y `pageSize`.

Ejemplo de asignación:

```bash
curl -X POST http://localhost:8080/api/users/USER_ID/roles/IB_ONCO_CONSULTA \
  -H "Authorization: Bearer ACCESS_TOKEN"
```

### Catálogos

Los endpoints de `/api/catalogos` administran tipos de unidad, municipios, localidades, unidades, tipologías y la relación entre tipologías y unidades. El acceso está restringido a `ADMIN_TIC`.

### Infraestructura

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/ping` | Comprueba que la API responde. |
| `GET` | `/health` | Health check para infraestructura. |

Swagger está disponible en desarrollo. La URL habitual es `http://localhost:8080/swagger`.

## Base de datos

`DbInitializer` crea de forma idempotente el esquema y los catálogos base al iniciar. También garantiza que `demo@example.com` conserve `ADMIN_TIC`.

Los scripts SQL de referencia viven en `database/scripts`.

El proyecto utiliza actualmente `EnsureCreatedAsync` y SQL idempotente. Si se adopta un flujo formal de migraciones:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project src/Infrastructure --startup-project src/WebAPI
dotnet ef database update --project src/Infrastructure --startup-project src/WebAPI
```

No mezcles migraciones con una base existente sin revisar primero el historial y el esquema generado.

## Seguridad

- BCrypt para contraseñas.
- JWT firmado y validado por issuer, audience, firma y expiración.
- Refresh tokens rotatorios de un solo uso.
- Rate limiting en login y refresh.
- Política `AdminTic` basada en el claim de rol `ADMIN_TIC`.
- CORS limitado a orígenes configurados.
- Middleware de errores con respuestas `ProblemDetails`.
- Headers `X-Content-Type-Options` y `X-Frame-Options`.

## Variables de entorno para despliegue

```text
DATABASE_URL=postgresql://user:pass@host:5432/db
JwtSettings__SecretKey=min-32-characters-strong-secret
JwtSettings__Issuer=imssb-bc-net-backend
JwtSettings__Audience=imssb-bc-net-frontend
ASPNETCORE_ENVIRONMENT=Production
CORS_ALLOWED_ORIGINS=https://tu-frontend.com
CORS_ALLOWED_ORIGIN_SUFFIXES=imssb-bc-gestion.netlify.app
```

`CORS_ALLOWED_ORIGIN_SUFFIXES` es opcional y solo admite subdominios HTTPS. Es útil para Deploy Previews de Netlify. No configures sufijos demasiado amplios como `netlify.app`.

## Pruebas

```bash
dotnet test
```

Las pruebas cubren credenciales inválidas, rotación de refresh tokens, cambio de contraseña, validación de la contraseña actual, restablecimiento administrativo y revocación de sesiones.

## Extensiones recomendadas para VS Code

- C# Dev Kit
- C# Extensions
- NuGet Gallery
- REST Client
- Docker
- GitLens
- EditorConfig for VS Code

## Monitoreo

Serilog escribe a consola, lo que permite que Render capture los eventos mediante `stdout`. Para ampliar observabilidad se puede integrar Sentry; OpenTelemetry con Grafana Cloud es apropiado cuando se necesiten trazas y métricas entre varios servicios.
