# Migración del backend Solicitudes (Node.js → .NET 8)

Los endpoints de `/api/auth` quedan fuera de alcance: la autenticación y los tokens permanecen en la implementación nativa de esta solución.

## Principios

- Se conservan rutas, verbos, parámetros y formas JSON para no romper clientes.
- Los módulos migrados requieren JWT y alguno de estos roles: `IB_ONCO`, `SOLICITUDES_ABASTO`, `ADMIN_TIC`, `COORDINACION` o `ABASTO`.
- El acceso a PostgreSQL se implementa exclusivamente con EF Core y LINQ; no se usa Dapper ni SQL incrustado en servicios/controladores.
- Las operaciones batch usan transacciones, `AddRange`/`ExecuteUpdate` y bloques acotados.
- El cuerpo HTTP se limita a 10 MiB antes de deserializar, igual que en Express.
- Los Excel se generan con ClosedXML y las respuestas usan compresión HTTP de ASP.NET Core.

## Capas

- `Application`: contratos y puertos.
- `Infrastructure`: PostgreSQL, Excel, integraciones y batch.
- `WebAPI`: contratos HTTP, autorización y controladores.

El modelo operativo usa `SolicitudesDbContext`, separado de `AppDbContext`. Esta separación evita que tablas de inventario, abasto y reportes se mezclen con el agregado de identidad, pero ambos contextos comparten la misma conexión PostgreSQL y unidad física de despliegue.

## Hallazgos del esquema existente

PostgreSQL permite las siguientes FKs entre enteros de distinto ancho, pero EF Core no puede representarlas como navegaciones:

| Tabla/columna | Tipo | Principal | Tipo |
|---|---:|---|---:|
| `dispositivo_nic.dispositivo_id` | `bigint` | `dispositivo.id` | `integer` |
| `kit_clave_unidad.kit_id` | `bigint` | `kit.id` | `integer` |
| `kit_clave_unidad.unidad_medica_id` | `bigint` | `unidad_medica.id` | `integer` |

Estas entidades se configurarán sin navegación automática y sus uniones usarán conversión numérica explícita en LINQ. La corrección ideal futura es homogeneizar cada par de columnas mediante una migración de base revisada y ejecutada por separado; la API nunca altera el esquema al iniciar.

## Estado de migración

- Infraestructura transversal: límite 10 MiB, compresión Brotli/Gzip y política `SolicitudesAccess` terminados.
- Modelo EF inicial: 55 entidades/tablas/vistas operativas generadas desde el catálogo real; falta incorporar manualmente las tres entidades afectadas por tipos de FK.
- Endpoints LINQ terminados: municipios, localidades, tipos de unidad y factores de conversión.
- Auth Node: excluido.

## Auth no se migra

No copiar servicios, rutas, controladores, Supabase, llaves o emisión de JWT del proyecto Node. Los módulos migrados consumen exclusivamente la identidad validada por ASP.NET Core.
