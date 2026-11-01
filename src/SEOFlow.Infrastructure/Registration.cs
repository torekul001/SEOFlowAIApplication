using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SEOFlow.Application;
using SEOFlow.Infrastructure.Data;
namespace SEOFlow.Infrastructure;
public static class Registration
{
    public static IServiceCollection AddSeoFlow(this IServiceCollection services, IConfiguration config, IHostEnvironment environment)
    {
        services.AddOptions<AuthOptions>().Bind(config.GetSection("Auth"))
            .Validate(o => Encoding.UTF8.GetByteCount(o.SigningKey) >= 32, "Auth:SigningKey requires at least 32 bytes of random key material.").ValidateOnStart();
        if (!environment.IsDevelopment() && string.IsNullOrWhiteSpace(config["Mail:Host"])) throw new InvalidOperationException("Production requires Mail:Host for verification and password reset delivery.");
        services.AddDbContext<SeoFlowDbContext>(o => o.UseSqlServer(config.GetConnectionString("SeoFlow") ?? throw new InvalidOperationException("Configure ConnectionStrings:SeoFlow using user-secrets or environment variables.")));
        services.AddScoped<IAuthService, AuthService>(); services.AddScoped<ITenantService, TenantService>();
        services.AddSingleton<IAccountMessageSink, AccountMessageSink>();
        services.AddHealthChecks().AddCheck<SqlHealthCheck>("sqlserver");
        return services;
    }
    public static TokenValidationParameters TokenValidation(IConfiguration config) => new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = config["Auth:Issuer"] ?? "SEOFlowAI",
        ValidAudience = config["Auth:Audience"] ?? "SEOFlowAI",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Auth:SigningKey"]!)),
        ClockSkew = TimeSpan.FromSeconds(20)
    };
    public static async Task<bool> IsCurrentUser(ClaimsPrincipal principal, SeoFlowDbContext db, CancellationToken ct)
    {
        return Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var id) && Guid.TryParse(principal.FindFirstValue("stamp"), out var stamp)
            && await db.Users.AnyAsync(x => x.Id == id && x.SecurityStamp == stamp && x.IsActive, ct);
    }
}
public sealed class SqlHealthCheck(SeoFlowDbContext db) : Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck
{
    public async Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try { return await db.Database.CanConnectAsync(cancellationToken) ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy() : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Database unavailable"); }
        catch { return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Database unavailable"); }
    }
}
public sealed class AccountMessageSink(IConfiguration config, IHostEnvironment environment) : IAccountMessageSink
{
    public async Task Deliver(string email, string purpose, string token, CancellationToken ct)
    {
        var host = config["Mail:Host"];
        if (string.IsNullOrWhiteSpace(host))
        {
            if (!environment.IsDevelopment()) throw new InvalidOperationException("Email delivery is not configured.");
            var directory = Path.Combine(environment.ContentRootPath, ".secrets", "outbox"); Directory.CreateDirectory(directory);
            // Development-only local outbox: never log bearer tokens or serve this directory.
            await File.WriteAllTextAsync(Path.Combine(directory, Guid.NewGuid() + ".txt"), $"To: {email}\nPurpose: {purpose}\nToken: {token}", ct); return;
        }
        using var smtp = new SmtpClient(host, config.GetValue("Mail:Port", 587)) { EnableSsl = true, Credentials = new NetworkCredential(config["Mail:Username"], config["Mail:Password"]) };
        using var message = new MailMessage(config["Mail:From"] ?? throw new InvalidOperationException("Mail:From missing"), email, "SEOFlow AI " + purpose, "Use this one-time token in SEOFlow AI within one hour:\n" + token);
        await smtp.SendMailAsync(message, ct);
    }
}
