SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM dbo.SchemaVersions WHERE Version=2)
BEGIN
CREATE TABLE dbo.Websites(Id uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(), TenantId uniqueidentifier NOT NULL REFERENCES dbo.Tenants(Id), Name nvarchar(160) NOT NULL, BaseUrl nvarchar(450) NOT NULL, CanonicalHost nvarchar(253) NOT NULL, IsVerified bit NOT NULL DEFAULT 0, IsDeleted bit NOT NULL DEFAULT 0, CreatedUtc datetime2 NOT NULL DEFAULT SYSUTCDATETIME(), RowVersion rowversion NOT NULL, CONSTRAINT UQ_Websites_TenantId_Id UNIQUE(TenantId,Id));
CREATE UNIQUE INDEX UX_Websites_Tenant_Host ON dbo.Websites(TenantId,CanonicalHost) WHERE IsDeleted=0;
INSERT dbo.SchemaVersions(Version) VALUES(2);
END
COMMIT;

