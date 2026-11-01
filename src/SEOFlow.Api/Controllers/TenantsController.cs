using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEOFlow.Application;
using SEOFlow.Contracts;
namespace SEOFlow.Api.Controllers;
[ApiController, Authorize, Route("api/v1/tenants")]
public sealed class TenantsController(ITenantService tenants) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    [HttpGet] public async Task<IActionResult> List(CancellationToken ct) => Ok(await tenants.List(UserId, ct));
    [HttpPost] public async Task<IActionResult> Create(CreateTenantRequest request, CancellationToken ct) { var id = await tenants.Create(UserId, request.Name, ct); return Created($"/api/v1/tenants/{id}/dashboard", new { id }); }
    [HttpGet("{tenantId:guid}/dashboard")] public async Task<IActionResult> Dashboard(Guid tenantId, CancellationToken ct) => Ok(await tenants.Dashboard(UserId, tenantId, ct));
    [HttpPost("{tenantId:guid}/invitations")] public async Task<IActionResult> Invite(Guid tenantId, InviteRequest request, CancellationToken ct) => Ok(new { token = await tenants.Invite(UserId, tenantId, request, ct), note = "Share this one-time token securely with the intended recipient." });
    [HttpPost("invitations/accept")] public async Task<IActionResult> Accept(AcceptInviteRequest request, CancellationToken ct) { await tenants.Accept(UserId, request.Token, ct); return NoContent(); }
    [HttpDelete("{tenantId:guid}/members/{memberId:guid}")] public async Task<IActionResult> Deactivate(Guid tenantId, Guid memberId, CancellationToken ct) { await tenants.Deactivate(UserId, tenantId, memberId, ct); return NoContent(); }
    [HttpGet("{tenantId:guid}/websites")] public async Task<IActionResult> Websites(Guid tenantId, int page = 1, int pageSize = 20, string? search = null, CancellationToken ct = default) => Ok(await tenants.Websites(UserId, tenantId, page, pageSize, search, ct));
    [HttpPost("{tenantId:guid}/websites")] public async Task<IActionResult> AddWebsite(Guid tenantId, CreateWebsiteRequest request, CancellationToken ct) { var id = await tenants.AddWebsite(UserId, tenantId, request, ct); return Created($"/api/v1/tenants/{tenantId}/websites", new { id }); }
}
