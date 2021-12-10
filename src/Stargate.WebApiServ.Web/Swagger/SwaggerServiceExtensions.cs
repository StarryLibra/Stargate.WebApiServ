using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Stargate.WebApiServ.Web.Swagger;

/// <summary>
/// Swagger 服务的扩展类。
/// </summary>
public static class SwaggerServiceExtensions
{
    /// <summary>
    ///  注册 Swagger 生成器配置，定义 Swagger 文档。
    /// </summary>
    /// <param name="services">依赖注入服务容器</param>
    /// <param name="authServer">认证服务器 URL 地址</param>
    /// <param name="ignoreObsolete">是否忽略已过时的操作与属性</param>
    /// <returns>添加了 Swagger 文档服务的依赖注入服务容器。</returns>
    public static IServiceCollection AddMySwaggerGen(this IServiceCollection services, Uri? authServer, bool ignoreObsolete = false)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "星门软件开发(StargateSoft Develop) WebAPI",
                Description = "星门软件开发(StargateSoft Develop)使用的示例微服务 WebAPI 接口文档 (版本1)",
                TermsOfService = new Uri("https://opensource.org/licenses/gpl-2.0.php"),
                Contact = new OpenApiContact { Name = "Libra", Email = "libra.zhu@hotmail.com", Url = new Uri("https://github.com/StarryLibra") },
                License = new OpenApiLicense { Name = "使用 MIT 许可协议", Url = new Uri("https://mit-license.org/") }
            });

            if (authServer is not null)
            {
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(authServer, "/connect/authorize"),
                            TokenUrl = new Uri(authServer, "/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                ["stargate_web-api-serv_read"] = "星门软件 WebAPI 读取服务",
                                ["stargate_web-api-serv_write"] = "星门软件 WebAPI 写入服务"
                            }
                        }
                    },
                    Description = "使用 password 方案进行用户身份验证",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" },
                            Scheme = "oauth2",
                            Name = "oauth2",
                            In = ParameterLocation.Header
                        },
                        Array.Empty<string>()
                    }
                });
            }

            var xmlDocFile = Path.Combine(AppContext.BaseDirectory, "SG-WebApiServ.xml");
            if (File.Exists(xmlDocFile))
                c.IncludeXmlComments(xmlDocFile);

            if (ignoreObsolete)     // 跳过已过时的操作与属性
            {
                c.IgnoreObsoleteActions();
                c.IgnoreObsoleteProperties();
            }

            // 跳过标记了 SwaggerExclude 特性的属性或字段
            c.SchemaFilter<SwaggerExcludePropertyFilter>();

            // 控制器（名称）分组增加文字描述
            c.DocumentFilter<TagDescriptionsDocumentFilter>();

            // 按控制器名称排序
            c.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
        });

        return services;
    }

    /// <summary>
    /// 登记Swagger文档服务中间件。
    /// </summary>
    /// <param name="app">中间件管道</param>
    /// <returns>添加了Swagger文档服务的中间件管道。</returns>
    public static IApplicationBuilder UseMySwaggerUI(this IApplicationBuilder app)
    {
        // 启用中间件处理 swagger-ui(HTML、JS、CSS等等)，特别是 Swagger JSON 终结点。
        app.UseSwaggerUI(c =>
        {
            c.DocumentTitle = "Swagger UI - Stargate WebApiServ";
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiServ v1");
            //c.DocExpansion(DocExpansion.None);

            c.OAuthClientId("web-api-serv_credential");
            c.OAuthClientSecret("secret_of_stargate");
            c.OAuthAppName("WebAPI Service");
            c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
        });

        return app;
    }
}
