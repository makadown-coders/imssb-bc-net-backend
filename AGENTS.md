# IMSS Bienestar BC Backend

## Stack

- .NET 8.
- ASP.NET Core Web API.
- PostgreSQL 17.
- Entity Framework Core 8.
- MediatR.
- FluentValidation.
- JWT con refresh tokens rotatorios de un solo uso.
- BCrypt para contraseñas.
- Serilog.
- Swagger/OpenAPI.
- Pruebas con `dotnet test`.

## Arquitectura

Mantener responsabilidades por proyecto:

- `Domain`: entidades y reglas del dominio. No depende de las demás capas.
- `Application`: casos de uso, comandos, queries, DTOs, interfaces y validaciones.
- `Infrastructure`: EF Core, PostgreSQL, repositorios y servicios externos.
- `WebAPI`: contratos HTTP, controladores, autorización y composición.
- `tests`: pruebas unitarias y de comportamiento crítico.

No introducir referencias de `Domain` hacia Infrastructure o WebAPI.
No ejecutar acceso a datos directamente desde controladores cuando exista o corresponda
un handler de Application.

## Reglas del dominio

- Persona representa el expediente humano u organizacional.
- Usuario representa una cuenta de acceso.
- Persona y Usuario mantienen una relación uno a uno.
- No crear un endpoint público para Usuarios independientes.
- `ADMIN_TIC` es un rol protegido.
- Asignar o revocar roles debe invalidar los refresh tokens correspondientes.
- Cambiar o restablecer una contraseña debe invalidar los refresh tokens.
- Los refresh tokens son rotatorios y de un solo uso.
- No debilitar las validaciones de contraseñas.
- La autorización real debe validarse en el backend.

## Base de datos

- No cambiar nombres de tablas, columnas, índices o relaciones sin señalar el impacto.
- No mezclar automáticamente `EnsureCreatedAsync`, SQL idempotente y migraciones.
- No crear una migración formal salvo que la tarea lo solicite y se haya revisado
  el estado actual del esquema.
- Evitar consultas N+1.
- Utilizar operaciones asincrónicas de EF Core.
- Propagar `CancellationToken` cuando el flujo existente lo permita.

## Contratos HTTP

- Mantener compatibilidad con rutas y cuerpos existentes salvo cambio explícito.
- Utilizar códigos HTTP apropiados.
- Mantener errores con `ProblemDetails`.
- Validar entradas con FluentValidation o el patrón existente.
- No exponer entidades de persistencia directamente cuando exista un DTO.

## Seguridad

- Nunca registrar contraseñas, JWT completos, refresh tokens o secretos.
- No incluir secretos predeterminados adecuados para producción.
- Mantener issuer, audience, firma y expiración del JWT.
- Revisar autorización y aislamiento de datos en cualquier endpoint nuevo.
- Agregar pruebas cuando se modifiquen autenticación, roles, contraseñas o sesiones.

## Comandos de validación

Restaurar:

    dotnet restore

Compilar:

    dotnet build --no-restore

Probar:

    dotnet test --no-build

Para una validación completa desde cero:

    dotnet restore
    dotnet build --no-restore
    dotnet test --no-build

## Code Review Rules

### Autenticación y sesiones

- Marcar cualquier cambio que permita reutilizar un refresh token.
- Marcar cambios de contraseña o roles que no invaliden las sesiones persistentes.
- Marcar endpoints administrativos sin política de autorización efectiva.

### Capas

- Marcar dependencias de Domain hacia Application, Infrastructure o WebAPI.
- Marcar acceso directo a EF Core desde controladores cuando corresponda un caso de uso.

### Persistencia

- Marcar cambios destructivos de esquema o adopción accidental de migraciones.
  Ruta segura: documentar el impacto y usar el mecanismo de inicialización acordado.