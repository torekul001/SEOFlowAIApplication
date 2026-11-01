using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SEOFlow.Infrastructure;
var builder = Host.CreateApplicationBuilder(args);
builder.Logging.ClearProviders(); builder.Logging.AddJsonConsole();
builder.Services.AddSeoFlow(builder.Configuration, builder.Environment);
builder.Services.AddHostedService<MaintenanceWorker>();
await builder.Build().RunAsync();
public sealed class MaintenanceWorker(ILogger<MaintenanceWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("SEOFlow worker foundation started. Scheduled SEO jobs become available in Phase 7.");
        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }
}
