using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// For more information on archive 'Implement background tasks in microservices with IHostedService and the BackgroundService class',
// visit https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/background-tasks-with-ihostedservice

#pragma warning disable CS1591
namespace Stargate.WebApiServ.Web.Services
{
    public class GracePeriodManagerService : BackgroundService
    {
        private int executionCount = 0;
        private readonly ILogger<GracePeriodManagerService> _logger;
        private readonly OrderingBackgroundSettings _settings;

        public GracePeriodManagerService(IOptions<OrderingBackgroundSettings> settings, ILogger<GracePeriodManagerService> logger)
        {
            _logger = logger;
            _settings = settings.Value;
            // Constructor's parameters validations...
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"GracePeriodManagerService is starting.");

            stoppingToken.Register(() =>
                _logger.LogDebug($"GracePeriod background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                if (executionCount <= 10)
                    _logger.LogDebug("GracePeriod task doing background work at {0:s}. ({1}th/top10 in LogDebug)", DateTime.UtcNow, executionCount);
                else
                    _logger.LogTrace("GracePeriod task doing background work at {0:s}.", DateTime.UtcNow);

                var count = Interlocked.Increment(ref executionCount);
                // do what you want

                await Task.Delay(_settings.CheckUpdateTime, stoppingToken);
            }

            _logger.LogDebug($"GracePeriod background task is stopping.");
        }
    }


    public class OrderingBackgroundSettings
    {
        public TimeSpan CheckUpdateTime { get; set; } = TimeSpan.FromMilliseconds(3000);
    }
}