using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SEOFlow.Application;
using SEOFlow.Contracts;
using SEOFlow.Web.Models;
namespace SEOFlow.Web.Controllers;
[Authorize]
public sealed class DashboardController(ITenantService tenants) : Controller
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    [HttpGet]
    public async Task<IActionResult> Index(Guid? tenantId, int page = 1, string? search = null, CancellationToken ct = default)
    {
        var list = await tenants.List(UserId, ct); tenantId ??= list.FirstOrDefault()?.Id;
        if (tenantId is null) return View(new DashboardModel(list, null, null, null));
        if (!list.Any(x => x.Id == tenantId)) return Forbid();
        return View(new DashboardModel(list, tenantId, await tenants.Dashboard(UserId, tenantId.Value, ct), await tenants.Websites(UserId, tenantId.Value, page, 10, search, ct)));
    }
    [HttpPost]
    public async Task<IActionResult> CreateTenant(CreateTenantRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) { TempData["Message"] = "Enter a valid organization name."; return RedirectToAction(nameof(Index)); }
        var id = await tenants.Create(UserId, request.Name, ct); return RedirectToAction(nameof(Index), new { tenantId = id });
    }
    [HttpPost]
    public async Task<IActionResult> AddWebsite(Guid tenantId, CreateWebsiteRequest request, CancellationToken ct)
    {
        try { if (!ModelState.IsValid) throw new ArgumentException("Enter a valid website name and HTTPS domain."); await tenants.AddWebsite(UserId, tenantId, request, ct); TempData["Message"] = "Website added. Ownership verification is available in Phase 2."; }
        catch (ArgumentException ex) { TempData["Message"] = ex.Message; }
        catch (DbUpdateException) { TempData["Message"] = "This domain already exists or the data changed. Refresh and try again."; }
        catch (UnauthorizedAccessException) { return Forbid(); }
        return RedirectToAction(nameof(Index), new { tenantId });
    }
    [HttpPost]
    public async Task<IActionResult> Invite(Guid tenantId, InviteRequest request, CancellationToken ct)
    {
        try { if (!ModelState.IsValid) throw new ArgumentException("Enter a valid email and role."); TempData["Message"] = "Share this one-time invitation token securely: " + await tenants.Invite(UserId, tenantId, request, ct); }
        catch (ArgumentException ex) { TempData["Message"] = ex.Message; }
        catch (UnauthorizedAccessException) { return Forbid(); }
        return RedirectToAction(nameof(Index), new { tenantId });
    }
    [HttpPost]
    public async Task<IActionResult> Accept(AcceptInviteRequest request, CancellationToken ct)
    {
        try { if (!ModelState.IsValid) throw new ArgumentException("Invitation token is required."); await tenants.Accept(UserId, request.Token, ct); TempData["Message"] = "Organization joined."; }
        catch (ArgumentException ex) { TempData["Message"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }
}
