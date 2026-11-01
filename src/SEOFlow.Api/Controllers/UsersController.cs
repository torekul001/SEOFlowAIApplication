using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEOFlow.Application;
namespace SEOFlow.Api.Controllers;
[ApiController, Authorize, Route("api/v1/users")]
public sealed class UsersController(ITenantService tenants) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    [HttpGet("me")] public IActionResult Me() => Ok(new { id = UserId, email = User.FindFirstValue(ClaimTypes.Email) });
    [HttpGet] public async Task<IActionResult> Members(Guid tenantId, int page = 1, int pageSize = 20, CancellationToken ct = default) => Ok(await tenants.Members(UserId, tenantId, page, pageSize, ct));
}
