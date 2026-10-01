# Consulta de grupos terapéuticos CNIS

API de solo lectura protegida con la política específica `CnisReadAccess`.
Requiere autenticación y al menos uno de estos roles: `ADMIN_TIC`, `IB_ONCO`,
`UNIDAD_MEDICA` o `ENFERMERIA`. Los roles `ABASTO`, `COORDINACION` y
`SOLICITUDES_ABASTO` por sí solos no conceden acceso a CNIS.
Esta política no modifica el acceso a Solicitudes ni concede permisos de escritura.
Los controladores envían queries MediatR a handlers de Application; el servicio
de Infrastructure ejecuta una consulta PostgreSQL por operación mediante EF Core 8.

## Rutas y ejemplos ilustrativos

`GET /api/cnis/grupos-terapeuticos`: solo grupos asociados a artículos existentes,
ordenados por número.

```json
[{"numero":16,"nombre":"Oncología"},{"numero":23,"nombre":"Cuidados Paliativos"}]
```

`GET /api/cnis/grupos-terapeuticos/articulos`: asociaciones distintas, ordenadas
por grupo y clave; conserva la relación N:N.

```json
[
  {"clave":"010.000.xxxx.xx","grupo":16,"grupoNombre":"Oncología"},
  {"clave":"010.000.xxxx.xx","grupo":23,"grupoNombre":"Cuidados Paliativos"}
]
```

`GET /api/cnis/grupos-terapeuticos/16/articulos`: artículos del grupo, ordenados
por clave, descripción y presentación. Un grupo inexistente o sin artículos
devuelve `200` con `[]`, siguiendo los filtros de catálogos operativos.

```json
[{"clave":"010.000.xxxx.xx","descripcion":"NOMBRE DEL MEDICAMENTO","presentacion":"Caja"}]
```

## Semántica y supuestos

La relación es `public.articulos.clave → cnis.insumo.clave_normalizada →
cnis.insumo_grupo → cnis.grupo`. PostgreSQL normaliza únicamente la clave del
artículo con `regexp_replace(trim(a.clave), '[^0-9]', '', 'g')` para el JOIN.
La respuesta conserva la clave original. No se utiliza `clavea`.

Descripción y presentación provienen de `public.articulos` y pueden ser nulas.
`DISTINCT` elimina filas idénticas; artículos con la misma clave pero diferentes
descripciones o presentaciones conservan esas diferencias. Los JOIN internos
excluyen claves sin CNIS e insumos sin asociación, incluido material de curación.

Se toma como contrato el esquema CNIS proporcionado; no se inspecciona ni altera
la base operativa durante las pruebas. El número de grupo se expone como entero.
No se agregan migraciones, tablas, vistas, dependencias de producción ni cambios
al esquema, a los datos o al mecanismo de inicialización.

## Validación

Se reutiliza el proyecto xUnit existente. Para ejecutar también las pruebas SQL,
definir `CNIS_TEST_CONNECTION_STRING` en el entorno apuntando a una instancia de
PostgreSQL de pruebas y ejecutar:

```text
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Las pruebas SQL usan un interceptor que sustituye las fuentes por CTEs con datos
ficticios; ejecutan en PostgreSQL las consultas del servicio, sin crear tablas ni
leer datos operativos. Cubren asociaciones simples y múltiples, normalización,
exclusión de insumos no relacionados, uso exclusivo de clave, deduplicación,
orden, origen de descripción/presentación, parámetros y cancelación. Si falta
la variable, estas pruebas se reportan como omitidas; la prueba de handlers
se ejecuta siempre.
