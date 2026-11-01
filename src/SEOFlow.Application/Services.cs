using SEOFlow.Contracts;
namespace SEOFlow.Application;
public interface IAuthService
{
    Task<Guid> Register(RegisterRequest request, CancellationToken ct);
    Task<AuthResult?> Login(LoginRequest request, CancellationToken ct);
    Task<AuthResult?> Refresh(string token, CancellationToken ct);
    Task Revoke(string token, CancellationToken ct);
    Task RequestAccountToken(string email, string purpose, CancellationToken ct);
    Task<bool> ConsumeAccountToken(string token, string purpose, string? password, CancellationToken ct);
}
public interface IAccountMessageSink { Task Deliver(string email, string purpose, string token, CancellationToken ct); }
public interface ITenantService
{
    Task<IReadOnlyList<TenantView>> List(Guid userId, CancellationToken ct);
    Task<Guid> Create(Guid userId, string name, CancellationToken ct);
    Task<string?> Role(Guid userId, Guid tenantId, CancellationToken ct);
    Task<string> Invite(Guid userId, Guid tenantId, InviteRequest request, CancellationToken ct);
    Task Accept(Guid userId, string token, CancellationToken ct);
    Task Deactivate(Guid userId, Guid tenantId, Guid memberId, CancellationToken ct);
    Task<PageResult<MemberView>> Members(Guid userId, Guid tenantId, int page, int size, CancellationToken ct);
    Task<DashboardView> Dashboard(Guid userId, Guid tenantId, CancellationToken ct);
    Task<PageResult<WebsiteView>> Websites(Guid userId, Guid tenantId, int page, int size, string? search, CancellationToken ct);
    Task<Guid> AddWebsite(Guid userId, Guid tenantId, CreateWebsiteRequest request, CancellationToken ct);
}
