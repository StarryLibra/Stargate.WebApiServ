using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Stargate.WebApiServ.Web.Swagger
{
    /// <summary>
    /// Swagger服务的扩展类。
    /// </summary>
    public static class SwaggerServiceExtensions
    {
        /// <summary>
        ///  注册Swagger生成器配置，定义Swagger文档。
        /// </summary>
        /// <param name="services">依赖注入服务容器</param>
        /// <param name="authServer">认证服务器URL地址</param>
        /// <param name="ignoreObsolete">是否忽略已过期的操作与属性</param>
        /// <returns>添加了Swagger文档服务的依赖注入服务容器。</returns>
        public static IServiceCollection AddMySwaggerGen(this IServiceCollection services, string authServer, bool ignoreObsolete = false)
        {
            return services.AddMySwaggerGen(new Action<SwaggerGenOptions>(options =>
            {
                options.AuthServer = new Uri(authServer);
                options.IgnoreObsolete = ignoreObsolete;
            }));
        }

        /// <summary>
        /// 注册Swagger生成器配置，定义Swagger文档。
        /// </summary>
        /// <param name="services">依赖注入服务容器</param>
        /// <param name="configureOptions">文档服务相应的配置和选项</param>
        /// <returns>添加了Swagger文档服务的依赖注入服务容器。</returns>
        public static IServiceCollection AddMySwaggerGen(this IServiceCollection services, Action<SwaggerGenOptions> configureOptions)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));

            services.Configure(configureOptions);
            
            var options = new SwaggerGenOptions();
            configureOptions.Invoke(options);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "星门软件开发(StargateSoft Develop) WebAPI",
                    Description = "星门软件开发(StargateSoft Develop)使用的示例微服务WebAPI接口文档 (版本1)",
                    TermsOfService = new Uri("https://opensource.org/licenses/gpl-2.0.php"),
                    Contact = new OpenApiContact { Name = "Libra", Email = "libra.zhu@hotmail.com", Url = new Uri("https://github.com/StarryLibra") },
                    License = new OpenApiLicense { Name = "使用MIT许可协议", Url = new Uri("https://mit-license.org/") }
                });

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(options.AuthServer, "/connect/authorize"),
                            TokenUrl = new Uri(options.AuthServer, "/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                ["stargate_web-api-serv_scope"] = "星门软件WebAPI服务"
                            }
                        }
                    },
                    Description = "使用 password 方案进行用户身份验证",
                });

                var xmlDocFile = Path.Combine(AppContext.BaseDirectory, "SG-WebApiServ.xml");
                if (File.Exists(xmlDocFile))
                    c.IncludeXmlComments(xmlDocFile);

                if (options.IgnoreObsolete)     // 跳过过期的操作与属性
                {
                    c.IgnoreObsoleteActions();
                    c.IgnoreObsoleteProperties();
                }

                // 跳过标记了SwaggerExclude特性的属性或字段
                c.SchemaFilter<SwaggerExcludePropertyFilter>();

                // Add security information to each operation for OAuth2
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                // 控制器（名称）分组增加文字描述
                c.DocumentFilter<TagDescriptionsDocumentFilter>();

                // 按控制器名称排序
                c.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
            });

            return services;
        }

        /// <summary>
        /// 注册Swagger生成器配置，定义Swagger文档。
        /// </summary>
        /// <param name="services">依赖注入服务容器</param>
        /// <param name="setupAction">配置文档服务的委托</param>
        /// <returns>添加了Swagger文档服务的依赖注入服务容器。</returns>
        public static IServiceCollection AddMySwaggerGen(this IServiceCollection services, Action<Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions> setupAction)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (setupAction == null)
                throw new ArgumentNullException(nameof(setupAction));

            return services.AddSwaggerGen(setupAction);
        }

        /// <summary>
        /// 登记Swagger文档服务中间件。
        /// </summary>
        /// <param name="app">中间件管道</param>
        /// <returns>添加了Swagger文档服务的中间件管道。</returns>
        public static IApplicationBuilder UseMySwaggerAndUI(this IApplicationBuilder app)
        {
            // 启用中间件处理Swagger生成的JSON终结点。
            app.UseSwagger();

            // 启用中间件处理swagger-ui(HTML、JS、CSS等等)，特别是Swagger JSON终结点。
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiServ v1");
                c.DocExpansion(DocExpansion.None);

                c.OAuthClientId("web-api-serv_credential");
                c.OAuthClientSecret("secret_of_stargate");
                c.OAuthAppName("WebAPI Service");
            });

            return app;
        }
    }
}
