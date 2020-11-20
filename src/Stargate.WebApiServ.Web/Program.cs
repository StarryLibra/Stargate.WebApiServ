using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

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
                .CreateLogger();

            try
            {
                Log.ForContext<Program>().Information("Starting web host...");
                CreateHostBuilder(args).Build().Run();
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
                    webBuilder.UseStartup<Startup>();
                });
    }
}
