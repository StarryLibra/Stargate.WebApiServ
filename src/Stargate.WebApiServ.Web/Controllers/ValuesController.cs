using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

// 关于通过 FormatFilter 进行内容协商参考博文“How to format response data as XML or JSON, based on the request URL in ASP.NET Core”
// 访问：https://andrewlock.net/formatting-response-data-as-xml-or-json-based-on-the-url-in-asp-net-core/

namespace Stargate.WebApiServ.Web.Controllers
{
    /// <summary>
    /// 微软Visual Studio 2019中新建控制器时内置的“包含读/写的API控制器”
    /// </summary>
    /// <remark>一个API控制器，其中包含用于创建、读取、更新、删除和列出条目的REST操作。</remark>
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        /// <summary>
        /// 列出条目。
        /// </summary>
        /// <returns>字符串组</returns>
        /// <remarks>
        /// 请求模式：
        /// GET /api/values
        /// </remarks>
        [HttpGet, FormatFilter]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// 读取。
        /// </summary>
        /// <param name="id">唯一标识值</param>
        /// <returns>字符串</returns>
        /// <remarks>
        /// 请求模式：
        /// GET /api/values/{id}
        /// </remarks>
        [HttpGet("{id}"), HttpGet("{id}.{format}"), FormatFilter]
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// 创建。
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <remarks>
        /// 请求模式：
        /// POST /api/values
        /// #string#
        /// </remarks>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        /// <summary>
        /// 更新。
        /// </summary>
        /// <param name="id">唯一标识值</param>
        /// <param name="value">字符串值</param>
        /// <remarks>
        /// 请求模式：
        /// PUT /api/values/{id}
        /// #string#
        /// </remarks>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        /// <summary>
        /// 删除。
        /// </summary>
        /// <param name="id">唯一标识值</param>
        /// <remarks>
        /// 请求模式：
        /// DELETE /api/values/{id}
        /// </remarks>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
