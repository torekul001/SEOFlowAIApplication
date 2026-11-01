$ErrorActionPreference = 'Stop'
# Configure the named connection through environment variables or API development user-secrets.
# API startup provides IConfiguration; secrets are not generated in DbContext.
dotnet tool restore
if ($LASTEXITCODE -ne 0) { throw 'Tool restore failed' }
dotnet ef dbcontext scaffold 'Name=ConnectionStrings:SeoFlow' Microsoft.EntityFrameworkCore.SqlServer --project src/SEOFlow.Infrastructure --startup-project src/SEOFlow.Api --output-dir Models --context-dir Data --context SeoFlowDbContext --no-onconfiguring --force
if ($LASTEXITCODE -ne 0) { throw 'Scaffolding failed' }

