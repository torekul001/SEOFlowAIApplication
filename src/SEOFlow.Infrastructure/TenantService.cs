using Dapper;
using Microsoft.EntityFrameworkCore;
using SEOFlow.Application;
using SEOFlow.Contracts;
using SEOFlow.Domain;
using SEOFlow.Infrastructure.Data;
using SEOFlow.Infrastructure.Models;
namespace SEOFlow.Infrastructure;
public sealed class TenantService(SeoFlowDbContext db) : ITenantService
{
    public async Task<IReadOnlyList<TenantView>> List(Guid userId, CancellationToken ct) => await db.TenantMemberships.Where(x => x.UserId == userId && x.IsActive && !x.Tenant.IsDeleted && x.User.IsActive).Select(x => new TenantView(x.TenantId, x.Tenant.Name, x.Role)).ToListAsync(ct);
    public async Task<string?> Role(Guid userId, Guid tenantId, CancellationToken ct) => await db.TenantMemberships.Where(x => x.UserId == userId && x.TenantId == tenantId && x.IsActive && !x.Tenant.IsDeleted && x.User.IsActive).Select(x => x.Role).SingleOrDefaultAsync(ct);
    private async Task Require(Guid userId, Guid tenantId, bool manage, CancellationToken ct)
    {
        var role = await Role(userId, tenantId, ct);
        if (role is null || (manage && !TenantPolicy.CanManage(role))) throw new UnauthorizedAccessException("Tenant access denied.");
    }
    public async Task<Guid> Create(Guid userId, string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 160) throw new ArgumentException("Organization name is required (maximum 160 characters).");
        if (!await db.Users.AnyAsync(x => x.Id == userId && x.IsActive, ct)) throw new UnauthorizedAccessException();
        var tenant = new Tenant { Id = Guid.NewGuid(), Name = name.Trim(), Slug = Guid.NewGuid().ToString("N"), CreatedUtc = DateTime.UtcNow };
        db.Tenants.Add(tenant); db.TenantMemberships.Add(new() { TenantId = tenant.Id, UserId = userId, Role = "AgencyAdmin", IsActive = true, CreatedUtc = DateTime.UtcNow });
        Audit(userId, tenant.Id, "Tenant.Created", tenant.Name); await db.SaveChangesAsync(ct); return tenant.Id;
    }
    public async Task<string> Invite(Guid userId, Guid tenantId, InviteRequest request, CancellationToken ct)
    {
        await Require(userId, tenantId, true, ct);
        if (!TenantPolicy.Roles.Contains(request.Role) || request.Role == "SuperAdmin") throw new ArgumentException("Invalid invitation role.");
        var raw = AuthService.NewToken(); db.Invitations.Add(new() { Id = Guid.NewGuid(), TenantId = tenantId, Email = request.Email.Trim().ToUpperInvariant(), Role = request.Role, TokenHash = AuthService.Hash(raw), ExpiresUtc = DateTime.UtcNow.AddDays(2), CreatedUtc = DateTime.UtcNow });
        Audit(userId, tenantId, "Member.Invited", request.Email); await db.SaveChangesAsync(ct); return raw;
    }
    public async Task Accept(Guid userId, string token, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);
        var hash = AuthService.Hash(token); var user = await db.Users.SingleAsync(x => x.Id == userId && x.IsActive, ct);
        var invite = await db.Invitations.SingleOrDefaultAsync(x => x.TokenHash == hash && x.Email == user.NormalizedEmail && x.AcceptedUtc == null && x.ExpiresUtc > DateTime.UtcNow && !x.Tenant.IsDeleted, ct) ?? throw new ArgumentException("Invalid or expired invitation.");
        if (await db.TenantMemberships.AnyAsync(x => x.UserId == userId && x.TenantId == invite.TenantId, ct)) throw new ArgumentException("Membership already exists. Contact the organization administrator.");
        db.TenantMemberships.Add(new() { TenantId = invite.TenantId, UserId = userId, Role = invite.Role, IsActive = true, CreatedUtc = DateTime.UtcNow }); invite.AcceptedUtc = DateTime.UtcNow;
        Audit(userId, invite.TenantId, "Member.Joined", invite.Role); await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
    }
    public async Task Deactivate(Guid userId, Guid tenantId, Guid memberId, CancellationToken ct)
    {
        await Require(userId, tenantId, true, ct);
        if (userId == memberId) throw new ArgumentException("Self-deactivation is not permitted.");
        var member = await db.TenantMemberships.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.UserId == memberId, ct) ?? throw new KeyNotFoundException();
        member.IsActive = false; Audit(userId, tenantId, "Member.Deactivated", memberId.ToString()); await db.SaveChangesAsync(ct);
    }
    public async Task<PageResult<MemberView>> Members(Guid userId, Guid tenantId, int page, int size, CancellationToken ct)
    {
        await Require(userId, tenantId, true, ct); page = Math.Max(1, page); size = Math.Clamp(size, 1, 100);
        var query = db.TenantMemberships.AsNoTracking().Where(x => x.TenantId == tenantId);
        var total = await query.CountAsync(ct);
        return new(await query.OrderBy(x => x.User.Email).Skip((page - 1) * size).Take(size).Select(x => new MemberView(x.UserId, x.User.Email, x.Role, x.IsActive)).ToListAsync(ct), page, size, total);
    }
    public async Task<DashboardView> Dashboard(Guid userId, Guid tenantId, CancellationToken ct)
    {
        await Require(userId, tenantId, false, ct);
        // Authorization is repeated in the SQL to prevent a membership revocation race.
        const string sql = "SELECT t.Name TenantName, (SELECT COUNT(*) FROM dbo.TenantMemberships WHERE TenantId=t.Id AND IsActive=1) Members, (SELECT COUNT(*) FROM dbo.Websites WHERE TenantId=t.Id AND IsDeleted=0) Websites, 'Not connected' Integrations FROM dbo.Tenants t WHERE t.Id=@tenantId AND t.IsDeleted=0 AND EXISTS(SELECT 1 FROM dbo.TenantMemberships m JOIN dbo.Users u ON u.Id=m.UserId WHERE m.TenantId=t.Id AND m.UserId=@userId AND m.IsActive=1 AND u.IsActive=1)";
        return await db.Database.GetDbConnection().QuerySingleOrDefaultAsync<DashboardView>(new CommandDefinition(sql, new { tenantId, userId }, cancellationToken: ct)) ?? throw new UnauthorizedAccessException();
    }
    public async Task<PageResult<WebsiteView>> Websites(Guid userId, Guid tenantId, int page, int size, string? search, CancellationToken ct)
    {
        await Require(userId, tenantId, false, ct); page = Math.Max(1, page); size = Math.Clamp(size, 1, 100);
        var query = db.Websites.AsNoTracking().Where(x => x.TenantId == tenantId && !x.IsDeleted && x.Tenant.TenantMemberships.Any(m => m.UserId == userId && m.IsActive && m.User.IsActive));
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Name.Contains(search) || x.CanonicalHost.Contains(search));
        var total = await query.CountAsync(ct);
        return new(await query.OrderBy(x => x.Name).ThenBy(x => x.Id).Skip((page - 1) * size).Take(size).Select(x => new WebsiteView(x.Id, x.Name, x.BaseUrl, x.IsVerified)).ToListAsync(ct), page, size, total);
    }
    public async Task<Guid> AddWebsite(Guid userId, Guid tenantId, CreateWebsiteRequest request, CancellationToken ct)
    {
        var role = await Role(userId, tenantId, ct); if (role is null || !TenantPolicy.CanWriteWebsite(role)) throw new UnauthorizedAccessException();
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 160) throw new ArgumentException("Website name is required.");
        var uri = DomainName.Normalize(request.Domain);
        var website = new Website { Id = Guid.NewGuid(), TenantId = tenantId, Name = request.Name.Trim(), BaseUrl = uri.AbsoluteUri, CanonicalHost = uri.IdnHost, CreatedUtc = DateTime.UtcNow };
        db.Websites.Add(website); Audit(userId, tenantId, "Website.Created", uri.IdnHost); await db.SaveChangesAsync(ct); return website.Id;
    }
    private void Audit(Guid user, Guid tenant, string action, string detail) => db.AuditLogs.Add(new() { UserId = user, TenantId = tenant, Action = action, Detail = detail, CreatedUtc = DateTime.UtcNow });
}
