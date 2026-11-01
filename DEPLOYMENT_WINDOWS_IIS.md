# Windows Server / IIS deployment

1. Install IIS, ASP.NET Core .NET 8 Hosting Bundle and a supported SQL Server. Restart IIS after installing the bundle. Provision HTTPS certificates and database/application service accounts. The application must not run as SQL sa.
2. Restore/build/test on the build host. Apply database scripts using a separate deployment principal after a verified SQL backup. Publish `dotnet publish src/SEOFlow.Api -c Release -o artifacts/publish/api` and `dotnet publish src/SEOFlow.Web -c Release -o artifacts/publish/web`.
3. Create separate IIS sites and app pools (No Managed Code), each pointing to its publish directory. Give the app pool identity read/execute access; allow write only to a protected log/key directory. Bind HTTPS hostnames. Set AllowedHosts accordingly. ASP.NET Core publish generates web.config for the IIS module.
4. Inject ConnectionStrings__SeoFlow, Auth__SigningKey, SMTP settings and ASPNETCORE_ENVIRONMENT=Production through protected deployment configuration. Never place secrets in source-controlled web.config. Persist MVC Data Protection keys using an ACL-protected location and certificate encryption; configure this explicitly before multiple instances/restarts. API/Web keys and issuer/audience must agree.
5. Run the operator bootstrap command once from a protected administrative terminal. Remove the one-time secret. Verify registration, SMTP verification/reset, login, tenant isolation, /health, cookie flags and HTTP-to-HTTPS behavior using real hostnames.
6. Configure application logging via IIS/Event Log or your log collector. Disable verbose SQL commands in production. Protect logs and set retention. Configure firewall access only from application hosts to SQL.

Worker: Phase 1 is a runnable host foundation and does not execute SEO jobs. Publish with `dotnet publish src/SEOFlow.Worker -c Release -o artifacts/publish/worker`; use a service manager or Windows Task Scheduler running the executable at startup with restart-on-failure and a least-privilege identity. A native Windows Service adapter and Hangfire persistence will be introduced with scheduled jobs; do not register this executable directly with sc.exe yet.

Rollback: stop affected app pools, restore the previous binaries, and use a reviewed compensating SQL migration or approved backup restore when schema compatibility requires it. Validate restored data in a separate environment first. No live SEO publishing exists in Phase 1, so website-content rollback is a Phase 6 feature.

This repository is production-oriented groundwork, not an assertion of production certification. Complete SECURITY.md release gates and all relevant later-phase acceptance tests before exposing live automation.
