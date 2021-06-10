using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stargate.WebApiServ.Data;
using Stargate.WebApiServ.Data.Models;

namespace Stargate.WebApiServ.Web.Controllers
{
    /// <summary>
    /// 示例用的人员控制器
    /// </summary>
    [Produces("application/json")]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {
        private readonly SampleContext _context;

        /// <summary>构造函数</summary>
        /// <param name="context">人员数据仓储</param>
        public PeopleController(SampleContext context)
        { 
            _context = context;
        }

        /// <summary>
        /// 获取人员列表。
        /// </summary>
        /// <returns>人员列表。</returns>
        [HttpGet]
        public ActionResult<IEnumerable<Person>> Get() =>
            _context.People.ToList();

        /// <summary>
        /// 根据 ID 获取人员
        /// </summary>
        /// <param name="id">人员唯一标识符</param>
        /// <returns>相应的人员</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetById(int id)
        {
            var person = await _context.People.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return person;
        }

        /// <summary>
        /// 新增人员
        /// </summary>
        /// <param name="person">新人员</param>
        /// <returns>新增后的人员</returns>
        [HttpPost]
        public async Task<ActionResult<Person>> Create(Person person)
        {
            _context.People.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
        }

        /// <summary>
        /// 更新现有人员。
        /// </summary>
        /// <param name="id">指定人员的唯一序列值</param>
        /// <param name="person">人员对象</param>
        /// <returns>处理更改的执行结果</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Person person)
        {
            if (id != person.Id)
            {
                return BadRequest();
            }

            _context.Entry(person).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// 删除人员。
        /// </summary>
        /// <param name="id">指定人员的唯一序列值</param>
        /// <returns>处理删除的执行结果</returns>
        /// <response code="204">空白内容</response>
        /// <response code="404">没有相应的水果</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            var person = await _context.People.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            _context.People.Remove(person);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
