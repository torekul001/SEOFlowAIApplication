using System.ComponentModel.DataAnnotations;
namespace SEOFlow.Contracts;
public sealed record RegisterRequest([Required, EmailAddress, MaxLength(256)] string Email, [Required, MinLength(12), MaxLength(128)] string Password, [Required, MaxLength(160)] string Organization);
public sealed record LoginRequest([Required, EmailAddress, MaxLength(256)] string Email, [Required, MaxLength(128)] string Password);
public sealed record RefreshRequest([Required, StringLength(96, MinimumLength = 96)] string RefreshToken);
public sealed record CreateTenantRequest([Required, MaxLength(160)] string Name);
public sealed record InviteRequest([Required, EmailAddress, MaxLength(256)] string Email, [Required] string Role);
public sealed record AcceptInviteRequest([Required, StringLength(96, MinimumLength = 96)] string Token);
public sealed record CreateWebsiteRequest([Required, MaxLength(160)] string Name, [Required, MaxLength(450)] string Domain);
public sealed record TokenRequest([Required, StringLength(96, MinimumLength = 96)] string Token);
public sealed record PasswordResetRequest([Required, StringLength(96, MinimumLength = 96)] string Token, [Required, MinLength(12), MaxLength(128)] string Password);
public sealed record EmailRequest([Required, EmailAddress] string Email);
public sealed record AuthResult(string AccessToken, string RefreshToken, DateTime ExpiresUtc);
public sealed record TenantView(Guid Id, string Name, string Role);
public sealed record WebsiteView(Guid Id, string Name, string BaseUrl, bool IsVerified);
public sealed record DashboardView(string TenantName, int Members, int Websites, string Integrations);
public sealed record PageResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int Total);


public sealed record MemberView(Guid UserId, string Email, string Role, bool IsActive);
