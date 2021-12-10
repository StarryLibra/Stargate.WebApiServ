// For more information on archive 'Background tasks with hosted services in ASP.NET Core',
// visit https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio
// sample code：https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/fundamentals/host/hosted-services/samples/3.x/BackgroundTasksSample

#pragma warning disable CS1591
namespace Stargate.WebApiServ.Web.Services;

/// <summary>
/// 计时的后台任务
/// </summary>
public class TimedHostedService : IHostedService, IDisposable
{
    private int executionCount = 0;
    private readonly ILogger<TimedHostedService> _logger;
    private Timer _timer = default!;

    /// <summary>构造函数</summary>
    /// <param name="logger">与计时的后台任务关联的应用日志</param>
    public TimedHostedService(ILogger<TimedHostedService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        _timer = new Timer(DoWork!, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(5));

        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        var count = Interlocked.Increment(ref executionCount);

        _logger.LogTrace(
            "Timed Hosted Service is working. Count: {Count}", count);
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _logger.LogInformation("Timed Hosted Service is disposing.");

        _timer?.Dispose();
    }
}
