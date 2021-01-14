using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Stargate.WebApiServ.Web.Swagger
{
    /// <summary>
    /// 向Swagger的分类项增加文字描述的IDocumentFilter过滤器。
    /// </summary>
    public class TagDescriptionsDocumentFilter : IDocumentFilter
    {
        /// <summary>
        /// 向分类项添加文字描述。
        /// </summary>
        /// <param name="swaggerDoc">Swagger的视图文档</param>
        /// <param name="context">Swagger内部的组织内容</param>
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
                    .Match(el.Attribute("name").Value)
                    .Groups["ctrler"]
                    .Value;
                var descr = descrReg.Replace(
                    el.Element("summary").Value,
                    String.Empty
                );
                swaggerDoc.Tags.Add(new OpenApiTag { Name = ctrler, Description = descr });
            }
        }
    }
}
