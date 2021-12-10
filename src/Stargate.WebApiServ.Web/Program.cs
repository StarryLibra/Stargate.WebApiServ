global using Microsoft.AspNetCore.Mvc;
global using Stargate.WebApiServ.Data;
global using Stargate.WebApiServ.Data.Models;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using LogDashboard;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using WebApiContrib.Core.Formatter.Yaml;
using Stargate.WebApiServ.Data.Repositories;
using Stargate.WebApiServ.Web.Libraries.JsonSerialization;
using Stargate.WebApiServ.Web.Services;
using Stargate.WebApiServ.Web.Swagger;

// 运行时 ASPNETCORE_ENVIRONMENT 环境变量建议值：Development-开发环境、Staging-预演环境、Production（或不设置）-生产环境
var currentEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
var config = new ConfigurationManager()
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
        p => p.StartsWith("/LogDashboard", StringComparison.OrdinalIgnoreCase) && p.Split('/')[^1].Contains('.')
    ))
    .WriteTo.File($"{AppContext.BaseDirectory}logs/LogDashboard.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:HH:mm:ss.fff} || {Level} || {SourceContext:l} || {Message} || {Exception} ||end {NewLine}"
    )
    .CreateLogger();

try
{
    Log.ForContext<Program>().Information("Starting web host...");
    
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    #region

    // Serilog logging for ASP.NET Core, visit https://github.com/serilog/serilog-aspnetcore
    builder.Host.UseSerilog();

    // Wait 30 seconds for graceful shutdown.
    builder.Host.ConfigureHostOptions(o => o.ShutdownTimeout = TimeSpan.FromSeconds(30));

    builder.Services.AddResponseCompression(options =>
    {
        options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/atom+xml", "image/svg+xml" });
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: "CorsPolicy",
            builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    });

    builder.Services.AddControllers(options =>
    {
        options.RespectBrowserAcceptHeader = true;
        options.ReturnHttpNotAcceptable = true;
    })
        .ConfigureApiBehaviorOptions(options =>
        {
            // 在请求中因模型验证失败而自动响应“HTTP 400 请求无效(Bad request)”错误时（此时会不进入任何控制器）插入日志记录。
            // 参考：https://github.com/dotnet/AspNetCore.Docs/issues/12157、 https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-5.0
            options.InvalidModelStateResponseFactory = context =>
            {
                Log.Error("请求的模型验证失败，响应 HTTP 400 请求无效(Bad request)错误。");

                var result = new BadRequestObjectResult(context.ModelState);
                result.ContentTypes.Add(MediaTypeNames.Application.Json);
                result.ContentTypes.Add(MediaTypeNames.Application.Xml);
                result.ContentTypes.Add("application/x-yaml");
                return result;
            };
        })
        .AddJsonOptions(options =>
        {
            var jsonConfig = config.GetSection("Json");

            if (jsonConfig.GetValue<string>("NamingPolicy").Equals("CamelCase", StringComparison.OrdinalIgnoreCase))
            {
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            }
            if (jsonConfig.GetValue("AllowNumberInQuotes", defaultValue: false))
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString;
            if (jsonConfig.GetValue("RelaxedEscaping", defaultValue: true))
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            options.JsonSerializerOptions.AllowTrailingCommas = jsonConfig.GetValue("AllowTrailingCommas", defaultValue: false);
            options.JsonSerializerOptions.DefaultIgnoreCondition =
                jsonConfig.GetValue("IgnoreNullValues", defaultValue: true)
                ? JsonIgnoreCondition.WhenWritingNull
                : JsonIgnoreCondition.Never;
            options.JsonSerializerOptions.WriteIndented = jsonConfig.GetValue("WriteIndented", defaultValue: false);

            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new MyDateTimeConverter());
            options.JsonSerializerOptions.Converters.Add(new MyDateTimeNullableConverter());
        })
        .AddXmlDataContractSerializerFormatters()
        .AddYamlFormatters();

    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));
    builder.Services.AddTransient<IContactRepository, ContactRepository>();
    builder.Services.AddScoped<ProductsRepository>();
    builder.Services.AddDbContext<ProductContext>(opt => opt.UseInMemoryDatabase("ProductInventory"));
    builder.Services.AddDbContext<SampleContext>(options => options.UseInMemoryDatabase("SampleData"));
    builder.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle.
    builder.Services.AddEndpointsApiExplorer();
    Uri.TryCreate(config["AppSettings:AuthServer"], UriKind.RelativeOrAbsolute, out var authServerUri);
    builder.Services.AddMySwaggerGen(authServerUri);

    builder.Services.AddHealthChecks();

    builder.Services.AddLogDashboard();

    // Note .AddMiniProfiler() returns a IMiniProfilerBuilder for easy intellisense
    builder.Services.AddMiniProfiler().AddEntityFramework();

    // 参考博客园博文《.NET Core 中正确使用 HttpClient 的姿势》，访问：https://www.cnblogs.com/willick/p/net-core-httpclient.html。
    //builder.Services.AddHttpClient();

    builder.Services.AddHostedService<TimedHostedService>();
    builder.Services.AddHostedService<GracePeriodManagerService>();
    //services.AddHostedService<GracefullyShutdownWorkerService>();
    #endregion

    var app = builder.Build();

    //app.Lifetime.ApplicationStarted.Register(() => Log.Information("Processing after application lifetime has started."));
    //app.Lifetime.ApplicationStopping.Register(() => Log.Information("Processing in application lifetime is on stopping."));
    //app.Lifetime.ApplicationStopped.Register(() => Log.Information("Processing after application lifetime had stoped."));

    // Configure the HTTP request pipeline.
    #region
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseMySwaggerUI();

        // 开发人员异常页面，参考：https://www.cnblogs.com/cool2feel/p/11453897.html
        app.UseDeveloperExceptionPage(new DeveloperExceptionPageOptions { SourceCodeLineCount = 10 });
    }
    else
    {
        app.UseExceptionHandler(builder => builder.Run(async context =>
        {
            var feature = context.Features.Get<IExceptionHandlerPathFeature>();
            var resp = new
            {
                Message = feature?.Error.Message ?? "全局异常捕获了一个未被处置的未明异常。",
                Path = feature?.Path ?? "未指明异常路径。",
                Source = feature?.Error.Source ?? "未指明异常抛出源。"
            };

            Log.Error(feature?.Error, resp.Message);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json; charset=utf-8";
            var jsonString = JsonSerializer.Serialize(resp);
            await context.Response.WriteAsync(jsonString);
        }));
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();

        app.UseResponseCompression();
    }

    // It's important that the UseSerilogRequestLogging() call appears before handlers such as MVC.
    // The middleware will not time or log components that appear before it in the pipeline. 
    app.UseSerilogRequestLogging(opts => {
        // 向请求日志添加扩展数据
        // 参考：https://www.cnblogs.com/yilezhu/p/12227271.html
        opts.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            // Set all the common properties available for every request
            diagnosticContext.Set("Host", httpContext.Request.Host);
            diagnosticContext.Set("Protocol", httpContext.Request.Protocol);
            diagnosticContext.Set("Scheme", httpContext.Request.Scheme);
            if (httpContext.Request.Headers.ContainsKey("Accept"))
                diagnosticContext.Set("Accept", httpContext.Request.Headers["Accept"].ToList());
            if (httpContext.Request.QueryString.HasValue)
                diagnosticContext.Set("QueryString", httpContext.Request.QueryString.Value);
            var endpoint = httpContext.GetEndpoint();
            if (endpoint is not null)
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            // Set the content-type of the response at this point
            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);
        };
        // 避免因健康检查(HealthCheck)请求导致过多日志。
        // 参考：https://www.cnblogs.com/yilezhu/p/12253361.html
        var defaultGetLevel = opts.GetLevel;
        opts.GetLevel = (ctx, elapsed, ex) =>
        {
            return ctx.GetEndpoint()?.DisplayName?.Equals("Health checks") ?? false
                ? LogEventLevel.Verbose
                : defaultGetLevel(ctx, elapsed, ex);
        };
    });

    app.UseHttpsRedirection();
    //app.UseStaticFiles();

    app.UseCors();

    // 防止反向代理多站点部署请求转发时路径丢失
    app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All });

    app.UseAuthorization();

    // 可视化日志面板，访问：https://doc.logdashboard.net 或 https://github.com/realLiangshiwei/LogDashboard
    app.UseLogDashboard();
    // The call to app.UseMiniProfiler must come before the call to app.UseEndpoints like
    app.UseMiniProfiler();
    // 借用欢迎页用于人工查看网站是否已正确启动
    app.UseWelcomePage("/welcome");

    app.MapGet("/", () => "Hello World!");
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        // 自定义健康检查(HealthCheck)的输出
        // 参考：https://www.cnblogs.com/yyfh/p/11787434.html
        ResponseWriter = (context, healthReport) =>
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                Status = healthReport.Status.ToString(),
                Errors = healthReport.Entries.Select(e => new { e.Key, Value = e.Value.Status.ToString() })
            }));
        }
    });
    app.MapControllers();
    #endregion

    SeedDatabase(app);

    app.Run();
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

static void SeedDatabase(IHost host)
{
    var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
    using var scope = scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<SampleContext>();
    if (context.Database.EnsureCreated())
    {
        try
        {
            SeedData.Initialize(context);
        }
        catch (Exception ex)
        {
            Log.ForContext<Program>().Error(ex, "A database seeding error occurred.");
        }
    }
}
