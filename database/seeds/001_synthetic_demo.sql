-- Optional synthetic fixture, never applied by migrations. OwnerUserId is supplied by the operator.
SET QUOTED_IDENTIFIER ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
DECLARE @owner uniqueidentifier = '$(OwnerUserId)';
IF NOT EXISTS(SELECT 1 FROM dbo.Users WHERE Id=@owner AND IsActive=1) THROW 50001, 'Register and verify the owner account first.', 1;
DECLARE @tenant uniqueidentifier = (SELECT Id FROM dbo.Tenants WHERE Slug='synthetic-demo' AND IsDeleted=0);
IF @tenant IS NULL
BEGIN
SET @tenant=NEWID();
INSERT dbo.Tenants(Id,Name,Slug) VALUES(@tenant,N'SYNTHETIC DEMO — Example Agency','synthetic-demo');
END
IF NOT EXISTS(SELECT 1 FROM dbo.TenantMemberships WHERE TenantId=@tenant AND UserId=@owner)
INSERT dbo.TenantMemberships(TenantId,UserId,Role) VALUES(@tenant,@owner,'AgencyAdmin');
IF NOT EXISTS(SELECT 1 FROM dbo.Websites WHERE TenantId=@tenant AND CanonicalHost='synthetic.example')
INSERT dbo.Websites(TenantId,Name,BaseUrl,CanonicalHost) VALUES(@tenant,N'SYNTHETIC DEMO — No real SEO metrics','https://synthetic.example/','synthetic.example');
COMMIT;
