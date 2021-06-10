using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stargate.WebApiServ.Data.Models;
using Stargate.WebApiServ.Data.Repositories;

// For more information on archive 'Use web API conventions',
// visit https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/conventions?view=aspnetcore-5.0
// sample code：https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/web-api/advanced/conventions/sample

#pragma warning disable API1000
namespace Stargate.WebApiServ.Web.Controllers
{
    /// <summary>
    /// 示例用的通讯录控制器（未应用 Web API 约定）
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactRepository _contacts;

        /// <summary>构造函数</summary>
        /// <param name="contacts">通讯录数据仓储</param>
        public ContactsController(IContactRepository contacts)
        {
            _contacts = contacts;
        }

        /// <summary>
        /// 获取通讯录。
        /// </summary>
        /// <returns>通讯录清单。</returns>
        /// <remarks>
        /// 请求模式：
        /// GET /api/contacts
        /// </remarks>
        // GET api/contacts
        [HttpGet]
        public IEnumerable<Contact> Get()
        {
            return _contacts.GetAll();
        }

        #region missing404docs
        /// <summary>
        /// 按 ID 获取某一通讯记录项。
        /// </summary>
        /// <param name="id">指定通讯记录项的唯一序列值</param>
        /// <returns>通讯记录项。</returns>
        /// <remarks>
        /// 请求模式：
        /// GET /api/contacts/{guid}
        /// 
        /// 本请求将会缺失状态码404（页面不存在）的返回约定。
        /// </remarks>
        // GET api/contacts/{guid}
        [HttpGet("{id}", Name = "GetContactsById")]
        [ProducesResponseType(typeof(Contact), StatusCodes.Status200OK)]
        public IActionResult Get(string id)
        {
            var contact = _contacts.Get(id);

            if (contact == null)
            {
                return NotFound();
            }

            return Ok(contact);
        }
        #endregion

        /// <summary>
        /// 添加新的通讯录记录项。
        /// </summary>
        /// <param name="contact">通讯录记录项</param>
        /// <returns>包含新的通讯录记录项的创建执行结果。</returns>
        /// <remarks>
        /// 请求模式：
        /// POST /api/contacts
        /// {
        ///   "firstName": #string#,
        ///   "lastName": #string#
        /// }
        /// </remarks>
        // POST api/contacts
        [HttpPost]
        public IActionResult Post(Contact contact)
        {
            _contacts.Add(contact);

            return CreatedAtRoute("GetContactsById", new { id = contact.ID }, contact);
        }

        /// <summary>
        /// 更新现有通讯录记录项。
        /// </summary>
        /// <param name="id">指定通讯录记录项的唯一序列值</param>
        /// <param name="contact">通讯录记录项</param>
        /// <returns>处理更改的执行结果</returns>
        /// <remarks>
        /// 请求模式：
        /// PUT /api/contacts/{guid}
        /// {
        ///   "id": #number#,
        ///   "firstName": #string#,
        ///   "lastName": #string#
        /// }
        /// </remarks>
        // PUT api/contacts/{guid}
        [HttpPut("{id}")]
        public IActionResult Put(string id, Contact contact)
        {
            if (ModelState.IsValid && id == contact.ID)
            {
                var contactToUpdate = _contacts.Get(id);

                if (contactToUpdate != null)
                {
                    _contacts.Update(contact);
                    return NoContent();
                }

                return NotFound();
            }

            return BadRequest();
        }

        /// <summary>
        /// 删除通讯录记录项。
        /// </summary>
        /// <param name="id">指定通讯录记录项的唯一序列值</param>
        /// <returns>处理删除的执行结果</returns>
        /// <remarks>
        /// 请求模式：
        /// DELETE /api/contacts/{id}
        /// </remarks>
        // DELETE api/contacts/{guid}
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var contact = _contacts.Get(id);

            if (contact == null)
            {
                return NotFound();
            }

            _contacts.Remove(id);

            return NoContent();
        }        
    }
}