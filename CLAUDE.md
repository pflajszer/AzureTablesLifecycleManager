# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A NuGet library (`AzureTablesLifecycleManager`) that adds lifecycle management — move/copy/delete/drop/query — to Azure Table Storage, which Azure's own Lifecycle Management doesn't cover. Published to nuget.org under GPL-3.0-or-later.

## Commands

```bash
DOTNET_ROLL_FORWARD=LatestMajor dotnet build AzureTablesLifecycleManager.sln  # see note below
dotnet test                                                    # all test projects
dotnet test AzureTablesLifecycleManager.Lib.Tests              # one project
dotnet test --filter "FullyQualifiedName~DropTablesAsync"       # one test / test group
dotnet pack AzureTablesLifecycleManager/AzureTablesLifecycleManager.csproj -c Release
```

**Building the full solution on a modern SDK needs `DOTNET_ROLL_FORWARD=LatestMajor`.** `Microsoft.NET.Sdk.Functions` 3.0.13 runs a build-time code generator (`Microsoft.NET.Sdk.Functions.Generator.dll`) that is itself a `netcoreapp3.1` app. That runtime is out of support and absent from current machines and CI runners, so without roll-forward the SystemTests project fails with `You must install or update .NET to run this application` — even though nothing is wrong with the code. The library and both test projects build without it; only SystemTests is affected.

**Azurite must be running before `dotnet test`.** Most tests are integration tests that hit a real storage account at `UseDevelopmentStorage=true` (set in `AzureTablesLifecycleManager.TestResources/ConfigConstants.cs`). They create and delete tables and rows for real. Some tests (e.g. `DropTablesAsync_EmptyFilter_...`) drop *every* table in the account — never point `ConnectionString` at anything but a local emulator.

`AzureTablesLifecycleManager.SystemTests` is a runnable Azure Functions v3 app used as a manual sample/smoke test; it consumes the library as a pinned NuGet `PackageReference`, not via project reference, so it does **not** exercise local changes — it demonstrates the last published version.

## Release

Releases are tag-driven: pushing a `v*` tag runs [.github/workflows/nuget-release.yml](.github/workflows/nuget-release.yml), which packs and pushes to nuget.org. The package version comes from the tag (`v2.2.0` → `2.2.0`), **not** from `<Version>` in the csproj, which is stale at `1.0.0`. Tags on `test` carry a `-beta` suffix; `master` tags are stable. The README's Change Log is maintained by hand per release.

## Architecture

Two layers, both registered as singletons by `AddAzureTablesLifecycleManagement()`:

- **`ITableManager` / `TableManager`** ([Lib/Services](AzureTablesLifecycleManager/Lib/Services/)) — the public API. Orchestrates multi-table operations: enumerate tables matching a table filter, then apply a data filter within each. `MoveDataBetweenTablesAsync` and `CopyDataFromTablesAsync` share private helpers that differ only by a `deleteSourceData` flag.
- **`ITableRepository` / `TableRepository`** ([AzureDAL/APIGateway](AzureTablesLifecycleManager/AzureDAL/APIGateway/)) — the only code that touches `Azure.Data.Tables`. New `TableManager` methods must go through this interface, never directly to the Azure SDK.

### Everything is dual-API: LINQ and OData

Every filtering operation ships two overloads — one taking `Expression<Func<T, bool>>` (LINQ), one taking `IQueryBuilder` (OData strings built with a fluent builder). This duality runs through the whole codebase and is the main thing to preserve when extending it:

- Adding an `ITableManager` method means adding **both** overloads, and both paths through `TableRepository`.
- Adding a predefined filter means adding it to **both** [ExpressionPredefinedFilters](AzureTablesLifecycleManager/Models/ExpressionPredefinedFilters.cs) and [ODataPredefinedFilters](AzureTablesLifecycleManager/Models/ODataPredefinedFilters.cs).

`QueryBuilder` is a mutable `StringBuilder` wrapper — it accumulates state across calls, so a DI-injected (singleton) instance must be `Flush()`ed between queries.

### `DataTransferResponse<T>`

Every `ITableManager` method returns this single accumulator type ([Models/DataTransferResponse.cs](AzureTablesLifecycleManager/Models/DataTransferResponse.cs)). It carries the queries that were run (both forms), the lazy `AsyncPageable` results, and a separate `List<Response>` per operation kind (`TableAdded`, `TableDeleted`, `DataAdded`, `DataUpdated`, `DataDeleted`). Success is not signalled by exceptions but by `AreOKResponses()` / `EnsureCorrectResponses()` in [DataTransferResponseExtensions](AzureTablesLifecycleManager/Lib/Extensions/DataTransferResponseExtensions.cs), which encode the expected HTTP status codes (`204` for most operations; `204` or `409` for table creation). These codes track the upstream `Azure.Data.Tables` library and have changed across versions — if you touch this, check the change log entry for v2.2.0.

Invalid new-table names are also non-throwing: the manager sets `IsNewTableNameValid = false` and returns early rather than raising. `EnsureValidAzureTableName()` on a string is the throwing variant (`^[A-Za-z][A-Za-z0-9]{2,62}$`).

### `TableRepository` caches `TableClient`s

The constructor enumerates the account and builds `_tableClients` once. Every entity-level method then does `_tableClients.Single(x => x.Name == tableName)`, which **throws `InvalidOperationException` if a table was created after the repository was constructed by anything other than this instance's own `CreateTable`/`CreateTableAsync`**. `DeleteTable`/`DeleteTableAsync` keep the cache in sync too. Tests that create tables out of band, or a second `TableRepository` sharing a storage account, hit this.

Bulk writes/deletes are issued as N individual requests fanned out with `Task.WhenAll` — not table transactions — so responses are per-entity and partial failure is possible.

## Conventions

- Target frameworks are `netcoreapp3.1;netstandard2.1`. Language features and BCL APIs must stay compatible with both; don't reach for newer C#/runtime APIs.
- Source is **tab-indented**, Allman braces, `_camelCase` private fields.
- Predefined filters are written as `static Func<...>` expression-bodied properties rather than methods — follow the existing shape when adding to those classes.
- Note the deliberate-looking but inconsistent namespace `AzureTablesLifecycleManagement.AzureDAL.Models` (…*Management*, not *Manager*) used by the OData operator constant classes in [Models/](AzureTablesLifecycleManager/Models/). Leave it alone unless doing a deliberate breaking change — it's part of the public API surface.
- `RegisterAzureTablesLifecycleManagement()` (on `IFunctionsHostBuilder`) is obsolete and slated for removal in v3.0.0; `AddAzureTablesLifecycleManagement()` (on `IServiceCollection`) replaces it. It reads the connection string from the `AzureWebJobsStorage` environment variable via `Environment.GetEnvironmentVariable`, not `IConfiguration`.
