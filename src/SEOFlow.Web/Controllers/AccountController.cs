using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEOFlow.Application;
using SEOFlow.Contracts;
using SEOFlow.Infrastructure;
namespace SEOFlow.Web.Controllers;
[Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("auth")]
public sealed class AccountController(IAuthService auth, IConfiguration config) : Controller
{
    [HttpGet] public IActionResult Login() => View();
    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(request);
        var result = await auth.Login(request, ct);
        if (result is null) { ModelState.AddModelError("", "Invalid credentials, unverified email, or account locked for 15 minutes."); return View(request); }
        var principal = new JwtSecurityTokenHandler().ValidateToken(result.AccessToken, Registration.TokenValidation(config), out _);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        // MVC uses a secure cookie, so the otherwise unused refresh-token family is revoked immediately.
        await auth.Revoke(result.RefreshToken, ct);
        return RedirectToAction("Index", "Dashboard");
    }
    [HttpGet] public IActionResult Register() => View();
    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(request);
        try { await auth.Register(request, ct); TempData["Message"] = "Account created. Verify your email to sign in."; return RedirectToAction(nameof(Verify)); }
        catch (ArgumentException ex) { ModelState.AddModelError("", ex.Message); return View(request); }
    }
    [HttpGet] public IActionResult Verify() => View();
    [HttpPost]
    public async Task<IActionResult> Verify(TokenRequest request, CancellationToken ct)
    {
        if (ModelState.IsValid && await auth.ConsumeAccountToken(request.Token, "VerifyEmail", null, ct)) { TempData["Message"] = "Email verified. You can sign in."; return RedirectToAction(nameof(Login)); }
        ModelState.AddModelError("", "Invalid or expired token."); return View(request);
    }
    [HttpGet] public IActionResult Forgot() => View();
    [HttpPost] public async Task<IActionResult> Forgot(EmailRequest request, CancellationToken ct) { if (!ModelState.IsValid) return View(request); await auth.RequestAccountToken(request.Email, "ResetPassword", ct); TempData["Message"] = "If the account exists, reset instructions have been sent."; return RedirectToAction(nameof(Reset)); }
    [HttpGet] public IActionResult Reset() => View();
    [HttpPost] public async Task<IActionResult> Reset(PasswordResetRequest request, CancellationToken ct) { if (ModelState.IsValid && await auth.ConsumeAccountToken(request.Token, "ResetPassword", request.Password, ct)) { TempData["Message"] = "Password reset. Sign in again."; return RedirectToAction(nameof(Login)); } ModelState.AddModelError("", "Invalid or expired token."); return View(request); }
    [Authorize, HttpPost] public async Task<IActionResult> Logout() { await HttpContext.SignOutAsync(); return RedirectToAction(nameof(Login)); }
    [HttpGet] public IActionResult Denied() { Response.StatusCode = 403; return View(); }
    [HttpGet] public IActionResult Error() { Response.StatusCode = 500; return View(); }
}
