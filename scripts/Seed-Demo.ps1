param([Parameter(Mandatory)][Guid]$OwnerUserId, [string]$Database='SEOFlowAI')
$ErrorActionPreference='Stop'
if ($Database -notmatch '^[A-Za-z][A-Za-z0-9_]{0,100}$') { throw 'Invalid database name' }
if (!$env:SEOFLOW_SQL_SERVER -or !$env:SEOFLOW_SQL_USER -or !$env:SQLCMDPASSWORD) { throw 'Set SQL deployment environment variables first.' }
# Configure a trusted SQL Server certificate; for development-only self-signed hosts, run sqlcmd manually with -C.
sqlcmd -S $env:SEOFLOW_SQL_SERVER -U $env:SEOFLOW_SQL_USER -d $Database -b -I -v "OwnerUserId=$OwnerUserId" -i (Join-Path $PSScriptRoot '../database/seeds/001_synthetic_demo.sql')
if ($LASTEXITCODE -ne 0) { throw 'Synthetic seed failed' }
