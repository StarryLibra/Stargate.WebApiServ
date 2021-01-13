using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
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
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

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
            services.AddMySwaggerGen(c =>
            {
                var authServerStr = this.Configuration["AppSettings:AuthServer"];
                if (!String.IsNullOrEmpty(authServerStr)
                  && Uri.TryCreate(authServerStr, UriKind.RelativeOrAbsolute, out var authServerUri))
                {
                    c.AuthServer = authServerUri;
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, ILogger<Startup> logger)
        {
            lifetime.ApplicationStarted.Register(() => logger.LogInformation("Started WebApiServ service lifetime."));
            lifetime.ApplicationStopping.Register(() => logger.LogWarning("Stopping WebApiServ service lifetime……"));
            lifetime.ApplicationStopped.Register(() => logger.LogWarning("Stoped WebApiServ service lifetime."));

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
            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthorization();

            app.UseWelcomePage("/welcome");     // 借用欢迎页用于人工测试网站是否已正确启动，必须在调用UseEndpoints()之前使用
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
