SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID('dbo.SchemaVersions') IS NULL CREATE TABLE dbo.SchemaVersions(Version int NOT NULL PRIMARY KEY, AppliedUtc datetime2 NOT NULL DEFAULT SYSUTCDATETIME());
IF NOT EXISTS(SELECT 1 FROM dbo.SchemaVersions WHERE Version=1)
BEGIN
CREATE TABLE dbo.Users(Id uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(), Email nvarchar(256) NOT NULL, NormalizedEmail nvarchar(256) NOT NULL UNIQUE, PasswordHash nvarchar(max) NOT NULL, EmailConfirmed bit NOT NULL DEFAULT 0, FailedAttempts int NOT NULL DEFAULT 0, LockoutEndUtc datetime2 NULL, SecurityStamp uniqueidentifier NOT NULL DEFAULT NEWID(), CreatedUtc datetime2 NOT NULL DEFAULT SYSUTCDATETIME(), IsActive bit NOT NULL DEFAULT 1, RowVersion rowversion NOT NULL);
CREATE TABLE dbo.Tenants(Id uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(), Name nvarchar(160) NOT NULL, Slug varchar(100) NOT NULL, CreatedUtc datetime2 NOT NULL DEFAULT SYSUTCDATETIME(), IsDeleted bit NOT NULL DEFAULT 0, RowVersion rowversion NOT NULL);
CREATE UNIQUE INDEX UX_Tenants_Slug ON dbo.Tenants(Slug) WHERE IsDeleted=0;
CREATE TABLE dbo.Roles(Name varchar(30) NOT NULL PRIMARY KEY);
INSERT dbo.Roles VALUES('SuperAdmin'),('AgencyAdmin'),('SEOManager'),('Editor'),('Analyst'),('ClientViewer');
CREATE TABLE dbo.TenantMemberships(TenantId uniqueidentifier NOT NULL REFERENCES dbo.Tenants(Id), UserId uniqueidentifier NOT NULL REFERENCES dbo.Users(Id), Role varchar(30) NOT NULL REFERENCES dbo.Roles(Name), IsActive bit NOT NULL DEFAULT 1, CreatedUtc datetime2 NOT NULL DEFAULT SYSUTCDATETIME(), RowVersion rowversion NOT NULL, PRIMARY KEY(TenantId,UserId));
CREATE INDEX IX_Memberships_User ON dbo.TenantMemberships(UserId,IsActive) INCLUDE(TenantId,Role);
CREATE TABLE dbo.RefreshTokens(Id uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(), UserId uniqueidentifier NOT NULL REFERENCES dbo.Users(Id), TokenHash varchar(64) NOT NULL UNIQUE, FamilyId uniqueidentifier NOT NULL, ExpiresUtc datetime2 NOT NULL, RevokedUtc datetime2 NULL, CreatedUtc datetime2 NOT NULL DEFAULT SYSUTCDATETIME(), RowVersion rowversion NOT NULL);
CREATE INDEX IX_RefreshTokens_Family ON dbo.RefreshTokens(FamilyId);
CREATE TABLE dbo.Invitations(Id uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(), TenantId uniqueidentifier NOT NULL REFERENCES dbo.Tenants(Id), Email nvarchar(256) NOT NULL, Role varchar(30) NOT NULL REFERENCES dbo.Roles(Name), TokenHash varchar(64) NOT NULL UNIQUE, ExpiresUtc datetime2 NOT NULL, AcceptedUtc datetime2 NULL, CreatedUtc datetime2 NOT NULL DEFAULT SYSUTCDATETIME(), RowVersion rowversion NOT NULL);
CREATE TABLE dbo.AccountTokens(Id uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(), UserId uniqueidentifier NOT NULL REFERENCES dbo.Users(Id), Purpose varchar(30) NOT NULL CHECK(Purpose IN('VerifyEmail','ResetPassword')), TokenHash varchar(64) NOT NULL UNIQUE, ExpiresUtc datetime2 NOT NULL, UsedUtc datetime2 NULL, CreatedUtc datetime2 NOT NULL DEFAULT SYSUTCDATETIME(), RowVersion rowversion NOT NULL);
CREATE TABLE dbo.AuditLogs(Id bigint IDENTITY PRIMARY KEY, TenantId uniqueidentifier NULL REFERENCES dbo.Tenants(Id), UserId uniqueidentifier NULL REFERENCES dbo.Users(Id), Action nvarchar(100) NOT NULL, Detail nvarchar(2000) NOT NULL, CreatedUtc datetime2 NOT NULL DEFAULT SYSUTCDATETIME());
CREATE INDEX IX_AuditLogs_Tenant_Time ON dbo.AuditLogs(TenantId,CreatedUtc DESC);
CREATE TABLE dbo.Permissions(Name varchar(100) NOT NULL PRIMARY KEY);
CREATE TABLE dbo.RolePermissions(Role varchar(30) NOT NULL REFERENCES dbo.Roles(Name), Permission varchar(100) NOT NULL REFERENCES dbo.Permissions(Name), PRIMARY KEY(Role,Permission));
INSERT dbo.Permissions VALUES('tenant.manage'),('tenant.read'),('website.write');
INSERT dbo.RolePermissions SELECT Name,'tenant.read' FROM dbo.Roles;
INSERT dbo.RolePermissions SELECT Name,'tenant.manage' FROM dbo.Roles WHERE Name IN('SuperAdmin','AgencyAdmin');
INSERT dbo.RolePermissions SELECT Name,'website.write' FROM dbo.Roles WHERE Name IN('SuperAdmin','AgencyAdmin','SEOManager');
INSERT dbo.SchemaVersions(Version) VALUES(1);
END
COMMIT;

