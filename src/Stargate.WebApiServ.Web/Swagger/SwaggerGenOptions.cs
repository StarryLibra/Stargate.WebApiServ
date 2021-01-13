using System;

namespace Stargate.WebApiServ.Web.Swagger
{
    /// <summary>
    /// Swagger文档服务相应的配置和选项。
    /// </summary>
    public class SwaggerGenOptions
    {
        /// <summary>
        /// 认证服务器URL地址
        /// </summary>
        /// <value>表示认证服务器URL地址的<c>Uri</c>对象。</value>
        public Uri AuthServer { get; set; } = new Uri("https://localhost/");

        /// <summary>
        /// 是否忽略已过期的操作与属性
        /// </summary>
        /// <value><c>true</c>表示可以忽略，反之<c>false</c>则表示需要包含已过期的操作与属性。默认值为：false。</value>
        public bool IgnoreObsolete { get; set; } = false;
    }
}
