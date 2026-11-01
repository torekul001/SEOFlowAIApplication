using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SEOFlow.Application;
using SEOFlow.Contracts;
using SEOFlow.Infrastructure;
using SEOFlow.Infrastructure.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders(); builder.Logging.AddJsonConsole();
builder.Services.AddSeoFlow(builder.Configuration, builder.Environment);
builder.Services.AddControllers(); builder.Services.AddEndpointsApiExplorer(); builder.Services.AddSwaggerGen(); builder.Services.AddProblemDetails();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = Registration.TokenValidation(builder.Configuration);
    o.Events = new JwtBearerEvents { OnTokenValidated = async context => { if (!await Registration.IsCurrentUser(context.Principal!, context.HttpContext.RequestServices.GetRequiredService<SeoFlowDbContext>(), context.HttpContext.RequestAborted)) context.Fail("Account no longer active"); } };
});
builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(o => { o.RejectionStatusCode = 429; o.AddPolicy("auth", context => RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "unknown", _ => new() { PermitLimit = builder.Configuration.GetValue("RateLimits:AuthPerMinute", 15), Window = TimeSpan.FromMinutes(1), QueueLimit = 0 })); });
var app = builder.Build();
app.UseExceptionHandler(error => error.Run(async context =>
{
    var ex = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    var status = ex switch { UnauthorizedAccessException => 403, ArgumentException => 400, KeyNotFoundException => 404, DbUpdateConcurrencyException => 409, DbUpdateException => 409, _ => 500 };
    await Results.Problem(statusCode: status, title: status == 500 ? "An unexpected error occurred." : status == 409 ? "The data changed or conflicts with an existing record." : ex?.Message, extensions: new Dictionary<string, object?> { ["traceId"] = context.TraceIdentifier }).ExecuteAsync(context);
}));
app.Use(async (context, next) => { context.Response.Headers["X-Content-Type-Options"] = "nosniff"; context.Response.Headers["Cache-Control"] = "no-store"; context.Response.Headers["X-Correlation-ID"] = context.TraceIdentifier; await next(); });
if (!app.Environment.IsDevelopment()) { app.UseHsts(); app.UseHttpsRedirection(); }
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseRateLimiter(); app.UseAuthentication(); app.UseAuthorization(); app.MapControllers(); app.MapHealthChecks("/health"); app.Run();
public partial class Program { }
