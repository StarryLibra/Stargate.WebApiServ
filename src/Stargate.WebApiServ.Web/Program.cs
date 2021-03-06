using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Filters;
using Stargate.WebApiServ.Data;

#pragma warning disable CS1591
namespace Stargate.WebApiServ.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 运行时 ASPNETCORE_ENVIRONMENT 环境变量建议值：Development-开发环境、Staging-预演环境、Production（或不设置）-生产环境
            var currentEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{currentEnv}.json", optional: true)
                .AddEnvironmentVariables(prefix: "WEBAPISERV_")
                .Build();

            // 为了能全生命周期（甚至在各种基础服务启动之前）记录日志，故早早在此处建立日志
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Filter.ByExcluding(Matching.WithProperty<string>(
                    "RequestPath",
                    p => p.StartsWith("/LogDashboard", StringComparison.OrdinalIgnoreCase) && p.Split('/')[^1].Contains(".")
                    ))
                .WriteTo.File($"{AppContext.BaseDirectory}logs/LogDashboard.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:HH:mm:ss.fff} || {Level} || {SourceContext:l} || {Message} || {Exception} ||end {NewLine}")
                .CreateLogger();

            try
            {
                Log.ForContext<Program>().Information("Starting web host...");
                var host = CreateHostBuilder(args).Build();
                SeedDatabase(host);
                host.Run();
            }
            catch (Exception ex)
            {
                Log.ForContext<Program>().Fatal(ex, "Host terminated unexpectedly!");
            }
            finally
            {
                Log.ForContext<Program>().Warning("Host had been closed.");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // Serilog logging for ASP.NET Core, visit https://github.com/serilog/serilog-aspnetcore
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseShutdownTimeout(TimeSpan.FromSeconds(5));     // 避免在主进程退出时服务能及时做相应处理
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(options => options.AllowSynchronousIO = true);
                });

        private static void SeedDatabase(IHost host)
        {
            var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<SampleContext>();

                if (context.Database.EnsureCreated())
                {
                    try
                    {
                        SeedData.Initialize(context);
                    }
                    catch (Exception ex)
                    {
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "A database seeding error occurred.");
                    }
                }
            }
        }
    }
}
