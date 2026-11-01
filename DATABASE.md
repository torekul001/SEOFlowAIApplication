# Database-first SQL Server

Ordered transactional, idempotent migrations:

- 001_identity_tenancy.sql: secure account storage, roles/permissions, tenants/memberships, refresh families, invitations, account-action tokens and audit logs.
- 002_websites.sql: tenant-owned website catalog, filtered unique domain index and composite resource key.
- 003_seo_modules.sql: forward schema for crawler/findings, provider metrics, keywords/ranks, content/approvals/publishing, scheduling/reports, plans/subscriptions/usage. These tables do not imply implemented product functionality.

Every version is recorded in SchemaVersions. Scripts use XACT_ABORT, explicit transactions, UTC defaults, foreign keys, unique/check constraints and rowversion. Website/tenant soft deletion is represented. Composite foreign keys carry TenantId through dependent resources. Do not edit an applied migration: author the next version. Run one deployment process at a time. SchemaVersions tracks versions, not cryptographic script checksums.

Apply with scripts/Apply-Database.ps1; take a verified backup first. scripts/Scaffold-Database.ps1 regenerates Models and Data from the live schema using a named environment configuration and --no-onconfiguring. Generated context must never contain a connection string. Keep service customizations out of generated files. Build and test after every scaffold.

The provided server was used only for a new SEOFlowAI application database and isolated disposable integration databases. Production uses a dedicated application principal with data permissions; schema deployment uses a separate DDL principal. No production sa connection belongs in deployment files.

Back up with SQL Server native BACKUP DATABASE to an operator-approved protected location, plus transaction log backups under full recovery. Validate restores on a separate SQL instance with DBCC CHECKDB and application smoke tests. Before a schema release take a backup and record SchemaVersions. Rollback uses a reviewed forward compensating migration or maintenance-window restore; no automatic down script drops customer data. Back up Data Protection keys and external encryption keys separately.

Provider secret columns are varbinary ciphertext only. No provider persistence code exists yet. A future adapter must encrypt via managed secret/key storage before writing and must not use SQL column type as a substitute for encryption.
