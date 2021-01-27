using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// 读取。
        /// </summary>
        /// <param name="id">唯一标识值</param>
        /// <returns>字符串</returns>
        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// 创建。
        /// </summary>
        /// <param name="value">字符串值</param>
        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        /// <summary>
        /// 更新。
        /// </summary>
        /// <param name="id">唯一标识值</param>
        /// <param name="value">字符串值</param>
        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        /// <summary>
        /// 删除。
        /// </summary>
        /// <param name="id">唯一标识值</param>
        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
