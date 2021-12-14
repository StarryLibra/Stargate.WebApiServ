// For more information on 'Separate readiness and liveness probes' section in archive 'Health checks in ASP.NET Core',
// visit https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0

using Microsoft.Extensions.Diagnostics.HealthChecks;

#pragma warning disable CS1591
namespace Stargate.WebApiServ.Web.Services;

/// <remarks>The background task simulates a startup process that takes roughly 15 seconds. Once it completes, the task sets the StartupHealthCheck.StartupCompleted property to true.</remarks>
public class StartupBackgroundService : BackgroundService
{
    private readonly StartupHealthCheck _healthCheck;

    public StartupBackgroundService(StartupHealthCheck healthCheck)
        => _healthCheck = healthCheck;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Simulate the effect of a long-running task.
        await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

        _healthCheck.StartupCompleted = true;
    }
}

/// <summary>
/// The StartupHealthCheck reports the completion of the long-running startup task and exposes the StartupCompleted property that gets set by the background service.
/// </summary>
public class StartupHealthCheck : IHealthCheck
{
    private volatile bool _isReady;

    public bool StartupCompleted
    {
        get => _isReady;
        set => _isReady = value;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (StartupCompleted)
            return Task.FromResult(HealthCheckResult.Healthy("The startup task has completed."));

        return Task.FromResult(HealthCheckResult.Unhealthy("That startup task is still running."));
    }
}
