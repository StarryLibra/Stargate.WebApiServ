using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LogDashboard;
using Serilog;
using Serilog.Events;
using Stargate.WebApiServ.Web.Models;
using Stargate.WebApiServ.Web.Swagger;

// 因Swagger需XML注释来完成WebAPI文档，故打开了项目级生成XML文档编译开关，但本文件无需关心XML注释
#pragma warning disable CS1591
namespace Stargate.WebApiServ.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            
            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    "application/atom+xml",
                    "image/svg+xml"
                });
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddHealthChecks();

            services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    var jsonConfig = Configuration.GetSection("Json");
                    if (jsonConfig.GetValue<string>("NamingPolicy").Equals("CamelCase", StringComparison.OrdinalIgnoreCase))
                    {
                        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    }
                    if (jsonConfig.GetValue<bool>("AllowNumberInQuotes", defaultValue: false))
                        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString;
                    if (jsonConfig.GetValue<bool>("RelaxedEscaping", defaultValue: true))
                        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                    options.JsonSerializerOptions.IgnoreNullValues = jsonConfig.GetValue<bool>("IgnoreNullValues", defaultValue: true);
                    options.JsonSerializerOptions.WriteIndented = jsonConfig.GetValue<bool>("WriteIndented", defaultValue: false);
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .AddMvcOptions(options =>
                {
                    options.RespectBrowserAcceptHeader = true;
                    options.ReturnHttpNotAcceptable = true;
                });
            services.AddMySwaggerGen(c =>
            {
                var authServerStr = this.Configuration["AppSettings:AuthServer"];
                if (!String.IsNullOrEmpty(authServerStr)
                  && Uri.TryCreate(authServerStr, UriKind.RelativeOrAbsolute, out var authServerUri))
                {
                    c.AuthServer = authServerUri;
                }
            });
            services.AddLogDashboard();

            services.PostConfigure<ApiBehaviorOptions>(options =>
            {
                // 在请求中因模型验证失败而自动响应HTTP 400请求无效(Bad request)错误时插入日志记录。
                // 参考：https://github.com/dotnet/AspNetCore.Docs/issues/12157
                var builtInFactory = options.InvalidModelStateResponseFactory;
                options.InvalidModelStateResponseFactory = context =>
                {
                    // Get an instance of ILogger and log accordingly.
                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger(context.ActionDescriptor.DisplayName);
                    logger.LogError("请求的模型验证失败，响应HTTP 400请求无效(Bad request)错误。");

                    return builtInFactory(context);
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, ILogger<Startup> logger)
        {
            lifetime.ApplicationStarted.Register(() => logger.LogInformation("Started WebApiServ service lifetime."));
            lifetime.ApplicationStopping.Register(() => logger.LogWarning("Be stopping WebApiServ service lifetime..."));
            lifetime.ApplicationStopped.Register(() => logger.LogWarning("Had Stoped WebApiServ service lifetime."));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMySwaggerAndUI();
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
                    
                    logger.LogError(feature?.Error, message: resp.Message);

                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json; charset=utf-8";
                    var jsonString = JsonSerializer.Serialize(resp);
                    await context.Response.WriteAsync(jsonString);
                }));

                app.UseResponseCompression();
            }

            app.UseHttpsRedirection();

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
                    if (endpoint is object)
                        diagnosticContext.Set("EndpointName", endpoint.DisplayName);
                    // Set the content-type of the response at this point
                    diagnosticContext.Set("ContentType", httpContext.Response.ContentType);
                };
                // 避免因健康检查(HealthCheck)请求导致过多日志。
                // 参考：https://www.cnblogs.com/yilezhu/p/12253361.html
                var defaultGetLevel = opts.GetLevel;
                opts.GetLevel = (ctx, elapsed, ex) =>
                {
                    return ctx.GetEndpoint()?.DisplayName.Equals("Health checks") ?? false
                        ? LogEventLevel.Verbose
                        : defaultGetLevel(ctx, elapsed, ex);
                };
            });

            app.UseRouting();
            app.UseCors();

            app.UseAuthorization();

            app.UseWelcomePage("/welcome");     // 借用欢迎页用于人工测试网站是否已正确启动，必须在调用UseEndpoints()之前使用
            app.UseLogDashboard();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
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
                endpoints.MapControllers();
            });
        }
    }
}
