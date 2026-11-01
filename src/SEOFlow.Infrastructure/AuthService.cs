using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SEOFlow.Application;
using SEOFlow.Contracts;
using SEOFlow.Infrastructure.Data;
using SEOFlow.Infrastructure.Models;
namespace SEOFlow.Infrastructure;
public sealed class AuthService(SeoFlowDbContext db, IOptions<AuthOptions> options, IAccountMessageSink messages) : IAuthService
{
    private readonly PasswordHasher<User> hasher = new(new OptionsWrapper<PasswordHasherOptions>(new() { IterationCount = 210000 }));
    public static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    public static string NewToken() => Convert.ToHexString(RandomNumberGenerator.GetBytes(48));
    public async Task<Guid> Register(RegisterRequest request, CancellationToken ct)
    {
        if (request.Password.Length is < 12 or > 128 || string.IsNullOrWhiteSpace(request.Organization)) throw new ArgumentException("Invalid registration.");
        var normalized = request.Email.Trim().ToUpperInvariant();
        if (await db.Users.AnyAsync(x => x.NormalizedEmail == normalized, ct)) throw new ArgumentException("Registration cannot be completed with these details.");
        var user = new User { Id = Guid.NewGuid(), Email = request.Email.Trim(), NormalizedEmail = normalized, IsActive = true, SecurityStamp = Guid.NewGuid(), CreatedUtc = DateTime.UtcNow };
        user.PasswordHash = hasher.HashPassword(user, request.Password);
        var tenant = new Tenant { Id = Guid.NewGuid(), Name = request.Organization.Trim(), Slug = Guid.NewGuid().ToString("N"), CreatedUtc = DateTime.UtcNow };
        db.Users.Add(user); db.Tenants.Add(tenant);
        db.TenantMemberships.Add(new() { TenantId = tenant.Id, UserId = user.Id, Role = "AgencyAdmin", IsActive = true, CreatedUtc = DateTime.UtcNow });
        db.AuditLogs.Add(new() { TenantId = tenant.Id, UserId = user.Id, Action = "Account.Registered", Detail = "Organization created", CreatedUtc = DateTime.UtcNow });
        await db.SaveChangesAsync(ct);
        await RequestAccountToken(user.Email, "VerifyEmail", ct);
        return user.Id;
    }
    public async Task<AuthResult?> Login(LoginRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToUpperInvariant();
        var user = await db.Users.SingleOrDefaultAsync(x => x.NormalizedEmail == email, ct);
        if (user is null) { hasher.VerifyHashedPassword(new User(), hasher.HashPassword(new User(), "TimingPaddingPassword"), request.Password); return null; }
        if (!user.IsActive || user.LockoutEndUtc > DateTime.UtcNow) return null;
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            user.FailedAttempts++;
            if (user.FailedAttempts >= 5) { user.LockoutEndUtc = DateTime.UtcNow.AddMinutes(15); user.FailedAttempts = 0; }
            await db.SaveChangesAsync(ct); return null;
        }
        if (options.Value.RequireVerifiedEmail && !user.EmailConfirmed) return null;
        user.FailedAttempts = 0; user.LockoutEndUtc = null;
        if (result == PasswordVerificationResult.SuccessRehashNeeded) user.PasswordHash = hasher.HashPassword(user, request.Password);
        return await Issue(user, Guid.NewGuid(), ct);
    }
    private async Task<AuthResult> Issue(User user, Guid family, CancellationToken ct)
    {
        var now = DateTime.UtcNow; var expires = now.AddMinutes(10); var raw = NewToken(); var o = options.Value;
        db.RefreshTokens.Add(new() { Id = Guid.NewGuid(), UserId = user.Id, FamilyId = family, TokenHash = Hash(raw), ExpiresUtc = now.AddDays(7), CreatedUtc = now });
        await db.SaveChangesAsync(ct);
        var jwt = new JwtSecurityToken(o.Issuer, o.Audience, [new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim("stamp", user.SecurityStamp.ToString()), new Claim(ClaimTypes.Email, user.Email)], now, expires, new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(o.SigningKey)), SecurityAlgorithms.HmacSha256));
        return new(new JwtSecurityTokenHandler().WriteToken(jwt), raw, expires);
    }
    public async Task<AuthResult?> Refresh(string token, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);
        var hash = Hash(token); var stored = await db.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == hash, ct);
        if (stored is null) return null;
        if (stored.RevokedUtc != null)
        {
            await db.RefreshTokens.Where(x => x.FamilyId == stored.FamilyId && x.RevokedUtc == null).ExecuteUpdateAsync(x => x.SetProperty(t => t.RevokedUtc, DateTime.UtcNow), ct);
            await tx.CommitAsync(ct); return null;
        }
        var user = await db.Users.SingleAsync(x => x.Id == stored.UserId, ct);
        if (stored.ExpiresUtc <= DateTime.UtcNow || !user.IsActive || user.LockoutEndUtc > DateTime.UtcNow) return null;
        stored.RevokedUtc = DateTime.UtcNow;
        var result = await Issue(user, stored.FamilyId, ct); await tx.CommitAsync(ct); return result;
    }
    public async Task Revoke(string token, CancellationToken ct)
    {
        var hash = Hash(token); var stored = await db.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == hash, ct);
        if (stored != null) await db.RefreshTokens.Where(x => x.FamilyId == stored.FamilyId && x.RevokedUtc == null).ExecuteUpdateAsync(x => x.SetProperty(t => t.RevokedUtc, DateTime.UtcNow), ct);
    }
    public async Task RequestAccountToken(string email, string purpose, CancellationToken ct)
    {
        var normalized = email.Trim().ToUpperInvariant(); var user = await db.Users.SingleOrDefaultAsync(x => x.NormalizedEmail == normalized && x.IsActive, ct);
        if (user is null) return;
        var raw = NewToken(); db.AccountTokens.Add(new() { Id = Guid.NewGuid(), UserId = user.Id, Purpose = purpose, TokenHash = Hash(raw), ExpiresUtc = DateTime.UtcNow.AddHours(1), CreatedUtc = DateTime.UtcNow });
        await db.SaveChangesAsync(ct); await messages.Deliver(user.Email, purpose, raw, ct);
    }
    public async Task<bool> ConsumeAccountToken(string token, string purpose, string? password, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);
        var hash = Hash(token); var stored = await db.AccountTokens.SingleOrDefaultAsync(x => x.TokenHash == hash && x.Purpose == purpose && x.UsedUtc == null && x.ExpiresUtc > DateTime.UtcNow, ct);
        if (stored is null) return false;
        var user = await db.Users.SingleAsync(x => x.Id == stored.UserId && x.IsActive, ct);
        if (purpose == "VerifyEmail") user.EmailConfirmed = true;
        else
        {
            if (password is null || password.Length is < 12 or > 128) return false;
            user.PasswordHash = hasher.HashPassword(user, password); user.SecurityStamp = Guid.NewGuid(); user.FailedAttempts = 0; user.LockoutEndUtc = null;
            await db.RefreshTokens.Where(x => x.UserId == user.Id && x.RevokedUtc == null).ExecuteUpdateAsync(x => x.SetProperty(t => t.RevokedUtc, DateTime.UtcNow), ct);
        }
        stored.UsedUtc = DateTime.UtcNow; await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); return true;
    }
}
