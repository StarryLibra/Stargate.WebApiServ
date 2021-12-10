// 获取文档《.NET Worker Service 如何优雅退出》更多的内容，
// 请访问 https://ittranslator.cn/dotnet/csharp/2021/05/17/worker-service-gracefully-shutdown.html

#pragma warning disable CS1591
namespace Stargate.WebApiServ.Web.Services;

public class GracefullyShutdownWorkerService : BackgroundService
{
    private bool _isStopping = false;   // 是否正在停止工作
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<GracefullyShutdownWorkerService> _logger;

    public GracefullyShutdownWorkerService(IHostApplicationLifetime hostApplicationLifetime, ILogger<GracefullyShutdownWorkerService> logger)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("上班了，又是精神抖擞的一天。 (Output from StartAsync)");
        return base.StartAsync(stoppingToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // 这里实现实际的业务逻辑
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Worker running at {time}. (Output from ExecuteAsync)", DateTimeOffset.Now);
                    await SomeMethodThatDoesTheWork(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Global exception occurred. Will resume in a moment. (Output from ExecuteAsync)");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
        finally
        {
            _logger.LogWarning("Exiting application... (Output from ExecuteAsync)");
            GetOffWork(stoppingToken);  // 关闭前需要完成的工作
            _hostApplicationLifetime.StopApplication();
        }
    }

    public override Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("太好了，下班时间到了。 (Output from StopAsync at {time})", DateTimeOffset.Now);
        _isStopping = true;

        _logger.LogInformation("去洗洗茶杯先……", DateTimeOffset.Now);
        Task.Delay(30_000).Wait();
        _logger.LogInformation("茶杯洗好了。", DateTimeOffset.Now);

        _logger.LogInformation("下班喽 ^_^", DateTimeOffset.Now);
        return base.StopAsync(stoppingToken);
    }

    private async Task SomeMethodThatDoesTheWork(CancellationToken cancellationToken)
    {
        if (_isStopping)
        {
            _logger.LogInformation("假装还在埋头苦干ing…… 其实我去洗杯子了");
            Task.Delay(TimeSpan.FromSeconds(1)).Wait();
        }
        else
        {
            _logger.LogInformation("我爱工作，埋头苦干ing……");
            Task.Delay(TimeSpan.FromSeconds(30)).Wait();
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// 关闭前需要完成的工作
    /// </summary>
    private void GetOffWork(CancellationToken cancellationToken)
    {
        _logger.LogInformation("啊，糟糕，有一个紧急 bug 需要下班前完成！！！");

        _logger.LogInformation("啊啊啊，我爱加班，我要再干10秒。(Wait 1)");
        Task.Delay(TimeSpan.FromSeconds(10)).Wait();
        _logger.LogInformation("啊啊啊啊啊啊，我爱加班，我要再干半分钟。(Wait 2)");
        Task.Delay(TimeSpan.FromMinutes(0.5)).Wait();

        _logger.LogInformation("啊哈哈哈哈哈，终于好了，下班走人！");
    }
}
