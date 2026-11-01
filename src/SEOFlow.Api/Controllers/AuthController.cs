using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SEOFlow.Application;
using SEOFlow.Contracts;
namespace SEOFlow.Api.Controllers;
[ApiController, Route("api/v1/auth"), EnableRateLimiting("auth")]
public sealed class AuthController(IAuthService auth) : ControllerBase
{
    [HttpPost("register")] public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct) { await auth.Register(request, ct); return StatusCode(201, new { message = "Account created. Verify your email before signing in." }); }
    [HttpPost("login")] public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct) { var result = await auth.Login(request, ct); return result is null ? Unauthorized(new ProblemDetails { Title = "Invalid credentials, unverified email, or locked account.", Status = 401 }) : Ok(result); }
    [HttpPost("refresh")] public async Task<IActionResult> Refresh(RefreshRequest request, CancellationToken ct) { var result = await auth.Refresh(request.RefreshToken, ct); return result is null ? Unauthorized() : Ok(result); }
    [HttpPost("revoke")] public async Task<IActionResult> Revoke(RefreshRequest request, CancellationToken ct) { await auth.Revoke(request.RefreshToken, ct); return NoContent(); }
    [HttpPost("request-verification")] public async Task<IActionResult> Verification(EmailRequest request, CancellationToken ct) { await auth.RequestAccountToken(request.Email, "VerifyEmail", ct); return Accepted(); }
    [HttpPost("forgot-password")] public async Task<IActionResult> Forgot(EmailRequest request, CancellationToken ct) { await auth.RequestAccountToken(request.Email, "ResetPassword", ct); return Accepted(); }
    [HttpPost("verify-email")] public async Task<IActionResult> Verify(TokenRequest request, CancellationToken ct) => await auth.ConsumeAccountToken(request.Token, "VerifyEmail", null, ct) ? NoContent() : BadRequest(new ProblemDetails { Title = "Invalid or expired token", Status = 400 });
    [HttpPost("reset-password")] public async Task<IActionResult> Reset(PasswordResetRequest request, CancellationToken ct) => await auth.ConsumeAccountToken(request.Token, "ResetPassword", request.Password, ct) ? NoContent() : BadRequest(new ProblemDetails { Title = "Invalid or expired token", Status = 400 });
}
