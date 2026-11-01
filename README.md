# SEOFlow AI

Runnable ASP.NET Core 8 / SQL Server foundation for a multi-tenant SEO agency platform. Phase 1 source is implemented; SEO crawling and provider integrations belong to subsequent phases. Dashboards show database counts and honest disconnected states.

## Run on Windows

For Visual Studio, use Visual Studio 2022 17.11 or newer with the ASP.NET and web development workload. The installed Visual Studio 17.4.3 cannot load this solution: its MSBuild 17.4 is below SDK 8.0.423's required MSBuild 17.8.3. Update through Visual Studio Installer; installing the standalone SDK alone does not update Visual Studio. Alternatively, run Run-SEOFlow.cmd without Visual Studio.

Install .NET SDK 8.0.423 and SQL Server command-line tools. Use SQL Server 2022 or compatible. SDK 10 was unavailable, so the master prompt's .NET 8 fallback is pinned in global.json.

1. `dotnet restore` and `dotnet tool restore`.
2. Set `SEOFLOW_SQL_SERVER`, `SEOFLOW_SQL_USER`, and `SQLCMDPASSWORD` in your terminal. Run `./scripts/Apply-Database.ps1`. For a development SQL host with a self-signed certificate only, add `-TrustServerCertificate`.
3. Set `ConnectionStrings__SeoFlow` and `Auth__SigningKey` (a cryptographically random secret of at least 32 bytes) in your terminal, or use project user-secrets. See ENVIRONMENT.md. Never use the SQL administrator account for production application access.
4. Set `$env:ASPNETCORE_ENVIRONMENT='Development'`.
5. In one terminal run `dotnet run --project src/SEOFlow.Api --urls http://localhost:5080`. Open `/swagger` for API exploration.
6. In another terminal with the same configuration run `dotnet run --project src/SEOFlow.Web --urls http://localhost:5081`. Open http://localhost:5081.
7. Register through the MVC page. Development verification/reset messages are written into the running host's `.secrets/outbox/` directory. Copy the token into the Verify email form. Sign in, select/create an organization, invite members, and add a website.

Optional: after registration, run `scripts/Seed-Demo.ps1 -OwnerUserId <your-user-guid>` with the SQL deployment environment variables to create a clearly labeled synthetic organization/site. Seeds are separate from migrations and contain no password or fake SEO measurements.

No paid provider keys are required. No synthetic SEO scores or provider metrics are displayed. API and MVC use the same database and domain services; MVC does not require the API process to be running. SQL connection and JWT key were configured in local user-secrets during initial implementation; these values are machine-local, excluded from this repository.

## Verify

`dotnet build SEOFlowAI.sln`

`dotnet test SEOFlowAI.sln`

SQL tests are explicitly skipped unless `SEOFLOW_TEST_SQL` is set to a connection with database creation rights. When set, they create an isolated `SEOFlowAI_Test_<guid>` database, apply scripts twice, exercise real HTTP/auth/tenant/database behavior, and drop only that generated database. Never point test setup at credentials that must not create disposable databases. Final verified result: 26 tests passed (18 unit, 8 real SQL integration), zero failed/skipped. See IMPLEMENTATION_STATUS.md. The Windows MVC smoke test can be rerun with `python scripts/Smoke-Mvc.py` after setting SQL deployment environment variables; it creates and removes its own isolated database.

## Bootstrap admin

Set `ConnectionStrings__SeoFlow`, `SEOFLOW_ADMIN_EMAIL`, and a random `SEOFLOW_BOOTSTRAP_SECRET` of at least 32 characters. Run `dotnet run --project tools/SEOFlow.Bootstrap` in an interactive terminal. Supply the matching secret and a unique initial password when prompted (both hidden). Bootstrap is refused after any SuperAdmin membership exists. Remove the bootstrap environment secret afterward. SuperAdmin remains tenant-scoped; it has no implicit access to customer organizations.

Read ARCHITECTURE.md, DATABASE.md, API.md, SECURITY.md, DEPLOYMENT_WINDOWS_IIS.md and IMPLEMENTATION_STATUS.md before extending or deploying.
