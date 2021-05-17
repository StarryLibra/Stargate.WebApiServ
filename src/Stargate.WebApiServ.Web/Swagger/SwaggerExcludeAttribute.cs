using System;

namespace Stargate.WebApiServ.Web.Swagger
{
    /// <summary>
    /// 用于指示在Swagger文档中跳过类/结构的某些属性或字段的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SwaggerExcludeAttribute : Attribute
    {
    }
}
