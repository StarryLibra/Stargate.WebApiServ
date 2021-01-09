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
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

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
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddHealthChecks();

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
                });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Stargate.WebApi", Version = "Ver 1.0" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiServ ver1.0"));
            }
            else
            {
                app.UseExceptionHandler(builder => builder.Run(async context =>
                {
                    var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var resp = new
                    {
                        Message = feature?.Error?.Message ?? "全局异常捕获了一个未被处置的未明异常。",
                        Path = feature?.Path,
                        Source = feature?.Error?.Source ?? "未指明异常抛出源。"
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
                // 避免因健康检查(HealthCheck)请求导致过多日志
                // 参考：https://www.cnblogs.com/yilezhu/p/12253361.html
                opts.GetLevel = (ctx, elapsed, ex) =>
                (
                    ctx.GetEndpoint()?.DisplayName.Equals("Health checks") ?? false
                        ? LogEventLevel.Verbose
                        : ex is not null
                            ? LogEventLevel.Error
                            : ctx.Response.StatusCode >= StatusCodes.Status500InternalServerError
                                ? LogEventLevel.Error
                                : LogEventLevel.Information
                );
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseWelcomePage("/welcome");     // 借用欢迎页用于人工测试网站是否已正确启动，必须在调用 UseEndpoints() 之前使用
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
