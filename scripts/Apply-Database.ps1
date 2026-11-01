param([string]$Server = $env:SEOFLOW_SQL_SERVER, [string]$Database = 'SEOFlowAI', [string]$User = $env:SEOFLOW_SQL_USER, [switch]$TrustServerCertificate)
$ErrorActionPreference = 'Stop'
if ($Database -notmatch '^[A-Za-z][A-Za-z0-9_]{0,100}$') { throw 'Invalid database name' }
if (!$Server -or !$User -or !$env:SQLCMDPASSWORD) { throw 'Set SEOFLOW_SQL_SERVER, SEOFLOW_SQL_USER and SQLCMDPASSWORD securely in this process.' }
$argsSql = @('-S',$Server,'-U',$User,'-b','-l','30','-I')
if ($TrustServerCertificate) { $argsSql += '-C' }
& sqlcmd @argsSql -Q "IF DB_ID(N'$Database') IS NULL CREATE DATABASE [$Database];"
if ($LASTEXITCODE -ne 0) { throw 'Database initialization failed' }
Get-ChildItem -LiteralPath (Join-Path $PSScriptRoot '../database/migrations') -Filter '*.sql' | Sort-Object Name | ForEach-Object {
    Write-Host "Applying $($_.Name)"
    & sqlcmd @argsSql -d $Database -i $_.FullName
    if ($LASTEXITCODE -ne 0) { throw "Migration failed: $($_.Name)" }
}
