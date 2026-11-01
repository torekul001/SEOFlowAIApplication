@echo off
setlocal
cd /d "%~dp0"
set ASPNETCORE_ENVIRONMENT=Development
echo Starting SEOFlow AI. Open http://localhost:5081 in your browser.
echo Keep this window open while using the application.
dotnet run --project src/SEOFlow.Web --urls http://localhost:5081
if errorlevel 1 pause
