<!--
Sync Impact Report
==================
Version change: (template, unratified) → 1.0.0
Rationale for bump: Initial ratification of the project constitution (MAJOR — establishes the
foundational governance for the first time).

Modified principles: n/a (initial adoption)
Added principles:
  - I. Framework Objetivo Único (.NET 8.0)
  - II. Integridad de la Arquitectura en Capas
  - III. Minimalismo de Dependencias
  - IV. Cambios Pequeños y Reversibles
Added sections:
  - Restricciones Técnicas y de Arquitectura (Section 2)
  - Flujo de Trabajo de Desarrollo (Section 3)
  - Governance (filled in)
Removed sections: none (all template placeholders replaced)

Deferred / TODO placeholders: none. RATIFICATION_DATE set to the date this constitution was first
adopted (no prior constitution existed — the file only contained unfilled template placeholders).

Templates requiring follow-up: none modified by this command (out of scope — dependent templates
and commands read this constitution at runtime, per the Scope Guard).
-->

# ShipmentTracker Constitution

## Core Principles

### I. Framework Objetivo Único (.NET 8.0)
Todos los proyectos de la solución (`ShipmentTracker.Core`, `ShipmentTracker.Infrastructure`,
`ShipmentTracker.Services`, `ShipmentTracker.Web`) DEBEN dirigirse exclusivamente a `net8.0`. Toda
dependencia, paquete NuGet o fragmento de código añadido DEBE ser compatible con .NET 8.0 sin
requerir multi-targeting ni `TargetFramework` divergentes entre proyectos. Cambiar el framework
objetivo (upgrade o downgrade) es una decisión arquitectónica que requiere una enmienda explícita a
esta constitución; no se realiza como efecto colateral de una tarea no relacionada.
Rationale: un framework objetivo único evita incompatibilidades de paquetes entre capas y mantiene
reproducible el build en todos los proyectos de la solución.

### II. Integridad de la Arquitectura en Capas
La solución mantiene la separación en capas ya establecida, con una única dirección de dependencia:
- `Core` (entidades, DTOs, enums, interfaces) NO depende de ningún otro proyecto de la solución.
- `Infrastructure` y `Services` dependen únicamente de `Core`.
- `Web` actúa como composition root y es el único proyecto autorizado a depender simultáneamente de
  `Core`, `Infrastructure` y `Services`.

Ningún cambio invierte este flujo (por ejemplo, `Core` referenciando `Infrastructure`, o `Services`
referenciando `Infrastructure` directamente). Los patrones ya adoptados —Repository, Unit of Work,
DTOs, inyección de dependencias por constructor— son la forma estándar de resolver sus respectivos
problemas. No se introduce un patrón alternativo que compita con uno ya adoptado para el mismo
propósito (p. ej. mapeo manual conviviendo con AutoMapper) sin retirar el enfoque anterior dentro del
mismo cambio.
Rationale: la regla de dependencia hacia `Core` es lo que hace testeable y reemplazable cada capa;
tolerar una sola excepción abre la puerta a que el resto del proyecto la repita.

### III. Minimalismo de Dependencias
No se añade ningún paquete NuGet ni dependencia externa salvo que resuelva una necesidad concreta que
no pueda cubrirse razonablemente con .NET 8.0/BCL o con las dependencias ya presentes (EF Core,
AutoMapper, FluentValidation, Swashbuckle/Swagger). Toda dependencia nueva propuesta DEBE justificarse
en la descripción del cambio: qué problema resuelve y por qué las dependencias existentes no bastan.
Rationale: cada dependencia añadida es superficie de mantenimiento, seguridad y compatibilidad
adicional; el listado actual ya cubre persistencia, mapeo, validación y documentación de API.

### IV. Cambios Pequeños y Reversibles
Los cambios se entregan en incrementos pequeños, cada uno revisable y revertible de forma
independiente. Se prohíben las reescrituras "big-bang" de una capa completa o de múltiples proyectos
en un solo cambio. Al corregir una inconsistencia puntual (p. ej. una estrategia de mapeo duplicada o
una dependencia resuelta con `new` en vez de inyección), el cambio se limita a esa inconsistencia,
sin aprovechar para refactorizar código no relacionado en el mismo commit o PR.
Rationale: cambios pequeños son más fáciles de revisar, probar y revertir sin arrastrar regresiones en
partes del sistema que no formaban parte del problema original.

## Restricciones Técnicas y de Arquitectura

- **Framework**: `net8.0` en los cuatro `.csproj` de la solución (`Core`, `Infrastructure`,
  `Services`, `Web`).
- **Mapa de dependencias permitido**: `Infrastructure → Core`, `Services → Core`,
  `Web → Core, Infrastructure, Services`. Ninguna otra flecha es válida.
- **Persistencia**: EF Core + SQL Server, encapsulado en `Infrastructure` (repositorios y
  `AppDbContext`). Ningún otro proyecto accede a `AppDbContext` o a un `DbSet` directamente; todo
  acceso a datos pasa por `IUnitOfWork` / `IBaseRepository<T>`.
- **Contrato HTTP**: la API se documenta con Swagger/OpenAPI y comentarios XML (`GenerateDocumentationFile`
  ya habilitado en `Core` y `Web`). Todo endpoint público nuevo o modificado incluye estos comentarios.
- **CORS**: la lista de orígenes permitidos en `Program.cs` es una allowlist explícita; no se
  reemplaza por `AllowAnyOrigin` ni equivalentes.

## Flujo de Trabajo de Desarrollo

- Cada cambio (commit o PR) aborda una sola inconsistencia o funcionalidad; no se combinan refactors
  no relacionados en el mismo cambio.
- Antes de añadir una dependencia nueva, se documenta en la descripción del cambio por qué las
  dependencias ya presentes (EF Core, AutoMapper, FluentValidation, Swashbuckle) no cubren la
  necesidad (ver Principio III).
- Un cambio que toque más de una capa a la vez requiere justificación explícita en su descripción,
  ya que el patrón esperado es que cada capa cambie de forma aislada gracias a las interfaces
  definidas en `Core`.
- El proyecto no cuenta actualmente con un proyecto de pruebas automatizadas. Si se incorpora, su
  adopción se hace de forma incremental (Principio IV) y no retroactiva a todo el código existente en
  un solo cambio; a partir de su incorporación, sus resultados pasan a ser parte obligatoria de la
  validación de cada cambio subsiguiente.

## Governance

Esta constitución prevalece sobre preferencias de estilo individuales o costumbres no documentadas
del equipo. Toda enmienda requiere: (1) descripción del cambio propuesto y su motivo, (2) actualización
de versión conforme a versionado semántico, (3) actualización de `Last Amended`. Un cambio que viole
un principio DEBE justificar la excepción explícitamente en su descripción, o ser rechazado.

**Versionado semántico de esta constitución**:
- MAJOR: eliminación o redefinición incompatible de un principio existente.
- MINOR: adición de un principio o sección nueva, o ampliación material de una guía existente.
- PATCH: aclaraciones de redacción o correcciones sin cambio de sentido.

**Version**: 1.0.0 | **Ratified**: 2026-08-16 | **Last Amended**: 2026-08-16
