using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using SEOFlow.Contracts;
using Xunit;
namespace SEOFlow.IntegrationTests;
public sealed class ApiTests(SqlFixture fixture) : IClassFixture<SqlFixture>
{
    [SqlFact]
    public async Task TenantACannotReadOrWriteTenantB()
    {
        var a = await fixture.Account("Tenant A"); var b = await fixture.Account("Tenant B");
        var add = await a.Client.PostAsJsonAsync($"/api/v1/tenants/{a.Tenant}/websites", new CreateWebsiteRequest("Own site", "example.com")); Assert.Equal(HttpStatusCode.Created, add.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await b.Client.GetAsync($"/api/v1/tenants/{a.Tenant}/dashboard")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await b.Client.GetAsync($"/api/v1/tenants/{a.Tenant}/websites")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await b.Client.PostAsJsonAsync($"/api/v1/tenants/{a.Tenant}/websites", new CreateWebsiteRequest("Attack", "evil.example"))).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await b.Client.GetAsync($"/api/v1/users?tenantId={a.Tenant}")).StatusCode);
        var members = await a.Client.GetFromJsonAsync<PageResult<MemberView>>($"/api/v1/users?tenantId={a.Tenant}"); Assert.Single(members!.Items);
        Assert.Equal(a.Email, members.Items[0].Email);
        var dashboard = await a.Client.GetFromJsonAsync<DashboardView>($"/api/v1/tenants/{a.Tenant}/dashboard"); Assert.Equal(1, dashboard!.Websites);
        var sites = await a.Client.GetFromJsonAsync<PageResult<WebsiteView>>($"/api/v1/tenants/{a.Tenant}/websites?search=Own"); Assert.Single(sites!.Items); Assert.False(sites.Items[0].IsVerified);
    }
    [SqlFact]
    public async Task RefreshRotatesAndReplayRevokesFamily()
    {
        var a = await fixture.Account("Rotation");
        var rotate = await a.Client.PostAsJsonAsync("/api/v1/auth/refresh", new RefreshRequest(a.Tokens.RefreshToken)); Assert.Equal(HttpStatusCode.OK, rotate.StatusCode);
        var next = (await rotate.Content.ReadFromJsonAsync<AuthResult>())!; Assert.NotEqual(a.Tokens.RefreshToken, next.RefreshToken);
        Assert.Equal(HttpStatusCode.Unauthorized, (await a.Client.PostAsJsonAsync("/api/v1/auth/refresh", new RefreshRequest(a.Tokens.RefreshToken))).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await a.Client.PostAsJsonAsync("/api/v1/auth/refresh", new RefreshRequest(next.RefreshToken))).StatusCode);
    }
    [SqlFact]
    public async Task InvitationEnforcesRecipientRoleAndDeactivation()
    {
        var admin = await fixture.Account("Agency"); var analyst = await fixture.Account("Analyst"); var other = await fixture.Account("Other");
        var invite = await admin.Client.PostAsJsonAsync($"/api/v1/tenants/{admin.Tenant}/invitations", new InviteRequest(analyst.Email, "ClientViewer")); invite.EnsureSuccessStatusCode();
        var json = await invite.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>(); var token = json.GetProperty("token").GetString()!;
        Assert.Equal(HttpStatusCode.BadRequest, (await other.Client.PostAsJsonAsync("/api/v1/tenants/invitations/accept", new AcceptInviteRequest(token))).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await analyst.Client.PostAsJsonAsync("/api/v1/tenants/invitations/accept", new AcceptInviteRequest(token))).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await analyst.Client.GetAsync($"/api/v1/tenants/{admin.Tenant}/dashboard")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await analyst.Client.PostAsJsonAsync($"/api/v1/tenants/{admin.Tenant}/websites", new CreateWebsiteRequest("Denied", "example.com"))).StatusCode);
        var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().ReadJwtToken(analyst.Tokens.AccessToken); var id = jwt.Claims.Single(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value;
        Assert.Equal(HttpStatusCode.NoContent, (await admin.Client.DeleteAsync($"/api/v1/tenants/{admin.Tenant}/members/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await analyst.Client.GetAsync($"/api/v1/tenants/{admin.Tenant}/dashboard")).StatusCode);
    }
    [SqlFact]
    public async Task PasswordResetInvalidatesJwtAndRefreshToken()
    {
        var a = await fixture.Account("Reset");
        (await a.Client.PostAsJsonAsync("/api/v1/auth/forgot-password", new EmailRequest(a.Email))).EnsureSuccessStatusCode();
        var reset = await a.Client.PostAsJsonAsync("/api/v1/auth/reset-password", new PasswordResetRequest(fixture.Messages.Tokens[a.Email + "ResetPassword"], "New-Strong-Password-456")); Assert.Equal(HttpStatusCode.NoContent, reset.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await a.Client.GetAsync("/api/v1/tenants")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await a.Client.PostAsJsonAsync("/api/v1/auth/refresh", new RefreshRequest(a.Tokens.RefreshToken))).StatusCode);
    }
    [SqlFact]
    public async Task VerificationIsSingleUseAndRequiredForLogin()
    {
        using var client = fixture.Factory.CreateClient(); var email = Guid.NewGuid() + "@example.com";
        (await client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest(email, "Strong-Test-Passphrase-123", "Verify"))).EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(email, "Strong-Test-Passphrase-123"))).StatusCode);
        var token = new TokenRequest(fixture.Messages.Tokens[email + "VerifyEmail"]);
        Assert.Equal(HttpStatusCode.NoContent, (await client.PostAsJsonAsync("/api/v1/auth/verify-email", token)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/v1/auth/verify-email", token)).StatusCode);
    }
    [SqlFact]
    public async Task FailedLoginsLockAccountAfterFiveAttempts()
    {
        var a = await fixture.Account("Lockout");
        for (var i = 0; i < 5; i++) Assert.Equal(HttpStatusCode.Unauthorized, (await a.Client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(a.Email, "Wrong-test-password"))).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await a.Client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(a.Email, "Strong-Test-Passphrase-123"))).StatusCode);
    }
    [SqlFact]
    public async Task AuthenticationRateLimitReturns429()
    {
        await using var factory = new ApiFactory(fixture.Connection, new TestMessages(), 3);
        using var client = factory.CreateClient();
        for (var i = 0; i < 3; i++) Assert.Equal(HttpStatusCode.Unauthorized, (await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest("nobody@example.com", "Invalid-password"))).StatusCode);
        Assert.Equal(HttpStatusCode.TooManyRequests, (await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest("nobody@example.com", "Invalid-password"))).StatusCode);
    }
    [SqlFact]
    public async Task InvalidRequestsAndAnonymousAccessAreRejected()
    {
        using var client = fixture.Factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/v1/tenants")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest("bad", "short", ""))).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health")).StatusCode);
    }
}
