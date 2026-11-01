# Architecture

Projects: Domain contains tenant-role policy and HTTPS domain normalization. Contracts contains validated request/response DTOs. Application owns service interfaces. Infrastructure provides SQL Server EF Core mappings scaffolded from SQL, Dapper dashboard reads, ASP.NET Core PasswordHasher-based authentication, JWT issuance, and account message delivery. Api hosts versioned REST controllers. Web hosts MVC/Razor with cookie authentication and local accessible responsive controls. Worker is a runnable host foundation; scheduling is not implemented. Bootstrap is an interactive operator-only command. UnitTests and IntegrationTests verify policy and real SQL/HTTP behavior.

Dependencies point toward Application/Domain. The infrastructure-generated persistence entities intentionally remain in Infrastructure. No EF migrations or EnsureCreated/EnsureDeleted calls are used. SQL is the schema source of truth.

Tenant IDs in routes select a resource; they never establish authority. Every service operation checks the authenticated user's active membership and active account. List queries filter tenant IDs explicitly. Dapper dashboard SQL repeats membership checks. Composite tenant/resource foreign keys in module schemas prevent cross-tenant relationships. SuperAdmin is a membership role, not a global authorization bypass.

Phase 1 UI: account lifecycle, tenant switcher/create/join, dashboard, searchable/paginated websites and invitations. Other requested SEO screens and provider adapters must be implemented in their corresponding phases; no fake links or success states stand in for them. A CSS design system is local with no commercial Kendo assets. Localization resources, Bootstrap/Kendo adapter work and advanced team administration remain tracked follow-ups.
