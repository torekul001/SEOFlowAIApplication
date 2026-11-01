using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SEOFlow.Application;
using SEOFlow.Contracts;
using Xunit;
[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace SEOFlow.IntegrationTests;
public sealed class SqlFactAttribute : FactAttribute
{
    public SqlFactAttribute() { if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SEOFLOW_TEST_SQL"))) Skip = "Set SEOFLOW_TEST_SQL to a SQL Server connection with CREATE DATABASE rights. Tests use an isolated disposable database."; }
}
public sealed class TestMessages : IAccountMessageSink
{
    public Dictionary<string, string> Tokens { get; } = new();
    public Task Deliver(string email, string purpose, string token, CancellationToken ct) { Tokens[email + purpose] = token; return Task.CompletedTask; }
}
public sealed class ApiFactory(string connection, TestMessages messages, int permitLimit = 1000) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, c) => c.AddInMemoryCollection(new Dictionary<string, string?> { ["ConnectionStrings:SeoFlow"] = connection, ["Auth:SigningKey"] = "integration-test-only-01234567890123456789", ["Auth:RequireVerifiedEmail"] = "true", ["RateLimits:AuthPerMinute"] = permitLimit.ToString() }));
        builder.ConfigureServices(s => { s.RemoveAll<IAccountMessageSink>(); s.AddSingleton<IAccountMessageSink>(messages); });
    }
}
public sealed class SqlFixture : IAsyncLifetime
{
    public ApiFactory Factory { get; private set; } = null!;
    public TestMessages Messages { get; } = new();
    public string Connection { get; private set; } = "";
    private string database = "";
    public async Task InitializeAsync()
    {
        var source = Environment.GetEnvironmentVariable("SEOFLOW_TEST_SQL"); if (string.IsNullOrEmpty(source)) return;
        database = "SEOFlowAI_Test_" + Guid.NewGuid().ToString("N");
        var cs = new SqlConnectionStringBuilder(source) { InitialCatalog = "master" };
        await using (var master = new SqlConnection(cs.ConnectionString)) { await master.OpenAsync(); await using var cmd = new SqlCommand($"CREATE DATABASE [{database}]", master); await cmd.ExecuteNonQueryAsync(); }
        cs.InitialCatalog = database; Connection = cs.ConnectionString;
        var root = new DirectoryInfo(AppContext.BaseDirectory); while (root != null && !Directory.Exists(Path.Combine(root.FullName, "database", "migrations"))) root = root.Parent;
        await using var db = new SqlConnection(Connection); await db.OpenAsync();
        foreach (var file in Directory.GetFiles(Path.Combine(root!.FullName, "database", "migrations"), "*.sql").Order()) { await using var cmd = new SqlCommand(await File.ReadAllTextAsync(file), db); await cmd.ExecuteNonQueryAsync(); }
        // A second run verifies migration idempotency against SQL Server.
        foreach (var file in Directory.GetFiles(Path.Combine(root!.FullName, "database", "migrations"), "*.sql").Order()) { await using var cmd = new SqlCommand(await File.ReadAllTextAsync(file), db); await cmd.ExecuteNonQueryAsync(); }
        Factory = new ApiFactory(Connection, Messages);
    }
    public async Task DisposeAsync()
    {
        if (Factory != null) await Factory.DisposeAsync();
        SqlConnection.ClearAllPools(); if (database.Length == 0) return;
        var cs = new SqlConnectionStringBuilder(Connection) { InitialCatalog = "master" };
        await using var db = new SqlConnection(cs.ConnectionString); await db.OpenAsync();
        if (!database.StartsWith("SEOFlowAI_Test_") || database.Length != 47) throw new InvalidOperationException("Unsafe test database name");
        await using var cmd = new SqlCommand($"ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{database}];", db); await cmd.ExecuteNonQueryAsync();
    }
    public async Task<(HttpClient Client, string Email, AuthResult Tokens, Guid Tenant)> Account(string name)
    {
        var client = Factory.CreateClient(); var email = Guid.NewGuid().ToString("N") + "@example.com";
        var register = await client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest(email, "Strong-Test-Passphrase-123", name)); register.EnsureSuccessStatusCode();
        var verify = await client.PostAsJsonAsync("/api/v1/auth/verify-email", new TokenRequest(Messages.Tokens[email + "VerifyEmail"])); verify.EnsureSuccessStatusCode();
        var login = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(email, "Strong-Test-Passphrase-123")); login.EnsureSuccessStatusCode();
        var tokens = (await login.Content.ReadFromJsonAsync<AuthResult>())!; client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        var tenants = (await client.GetFromJsonAsync<List<TenantView>>("/api/v1/tenants"))!; return (client, email, tokens, tenants[0].Id);
    }
}

