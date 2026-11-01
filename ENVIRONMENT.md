# Configuration

ASP.NET Core loads appsettings, Development user-secrets, environment variables, then command-line overrides. `.env.example` lists placeholders and is not automatically loaded.

Use `dotnet user-secrets set 'ConnectionStrings:SeoFlow' '<connection>' --project src/SEOFlow.Api` and repeat for Web and Worker. Configure `Auth:SigningKey` the same way (cryptographically random minimum 32 bytes). These commands are examples: do not copy real secrets into scripts or shell history in shared environments. Prefer environment injection from a deployment secret store.

Environment names: `ConnectionStrings__SeoFlow`, `Auth__SigningKey`, `Auth__Issuer`, `Auth__Audience`, `Auth__RequireVerifiedEmail` (true by default). Set ASPNETCORE_ENVIRONMENT=Development for API/Web; DOTNET_ENVIRONMENT=Development for Worker. Use production environment outside a developer machine.

Production SMTP: `Mail__Host`, `Mail__Port` (587 default), `Mail__From`, `Mail__Username`, `Mail__Password`. TLS is required. Production startup refuses a missing SMTP host; verify actual delivery before launch. Development without SMTP writes action tokens to a local non-public `.secrets/outbox` directory. Restrict that directory to your OS account and remove expired messages.

`SEOFLOW_TEST_SQL` enables isolated real SQL integration tests. `SEOFLOW_BOOTSTRAP_SECRET` plus `SEOFLOW_ADMIN_EMAIL` are consumed only by the interactive bootstrap tool. Bootstrap passwords are entered without echo; no default credentials exist. No OpenAI/Google/SERP/billing credentials are needed for Phase 1.

Use a trusted SQL certificate and Encrypt=True; TrustServerCertificate=True was used only to connect to the supplied development server. Bind AllowedHosts to deployed hostnames. Data Protection keys for MVC must persist securely across restarts and be shared only between replicas of the same app.
