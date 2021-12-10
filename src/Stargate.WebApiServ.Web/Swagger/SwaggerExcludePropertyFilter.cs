using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Stargate.WebApiServ.Web.Swagger;

/// <summary>
/// 在 Swagger 文档中跳过标记了 <c>SwaggerExcludeAttribute</c> 特性的 <c>ISchemaFilter</c> 过滤器。
/// </summary>
public class SwaggerExcludePropertyFilter : ISchemaFilter
{
    /// <summary>
    /// 跳过标记了 <c>SwaggerExcludeAttribute</c> 特性的模式内容。
    /// </summary>
    /// <param name="schema">Swagger 的模式</param>
    /// <param name="context">Swagger 模式过滤器的上下文</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var excludedProperties = context.Type.GetProperties();
        foreach (var property in excludedProperties)
        {
            var propKey = property.Name[0..1].ToLower() + property.Name[1..];

            var excludeAttributes = property.GetCustomAttributes(true).OfType<SwaggerExcludeAttribute>();
            if (excludeAttributes.Any() && schema.Properties.ContainsKey(propKey))
            {
                schema.Properties.Remove(propKey);
            }
        }
    }
}
