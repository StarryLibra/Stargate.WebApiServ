using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Stargate.WebApiServ.Web.Swagger;

/// <summary>
/// 向 Swagger 的分类项增加文字描述的 <c>IDocumentFilter</c> 过滤器。
/// </summary>
public class TagDescriptionsDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// 向分类项添加文字描述。
    /// </summary>
    /// <param name="swaggerDoc">Swagger 的视图文档</param>
    /// <param name="context">Swagger 文档过滤器的上下文</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var xmlDocFilePath = Path.Combine(AppContext.BaseDirectory, "SG-WebApiServ.xml");
        if (!File.Exists(xmlDocFilePath))
        {
            return;
        }
        var xdoc = XDocument.Load(xmlDocFilePath);

        var ctrlReg = new Regex(@"^T:Stargate.WebApiServ.Web.Controllers.(?<ctrler>\w+?)(Controller)?$");
        var descrReg = new Regex(@"\n(\s*)");

        swaggerDoc.Tags = new List<OpenApiTag>();
        var elements = xdoc.XPathSelectElements("/doc/members/member[starts-with(@name,'T:Stargate.WebApiServ.Web.Controllers.')]");
        foreach (var el in elements)
        {
            var ctrler = ctrlReg
                .Match(el.Attribute("name")?.Value ?? String.Empty)
                .Groups["ctrler"]
                .Value;
            var descr = descrReg.Replace(
                el.Element("summary")?.Value ?? String.Empty,
                String.Empty
            );
            swaggerDoc.Tags.Add(new OpenApiTag { Name = ctrler, Description = descr });
        }
    }
}
