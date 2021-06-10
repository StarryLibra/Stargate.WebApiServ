using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stargate.WebApiServ.Data.Models;
using Stargate.WebApiServ.Data.Repositories;

// For more information on archive 'Use web API conventions',
// visit https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/conventions?view=aspnetcore-5.0
// sample code：https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/web-api/advanced/conventions/sample

namespace Stargate.WebApiServ.Web.Controllers
{
    #region snippet_ApiConventionTypeAttribute
    /// <summary>
    /// 示例用的通讯录控制器（应用 Web API 约定）
    /// </summary>
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("api/[controller]")]
    public class ContactsConventionController : ControllerBase
    {
        #endregion
        private readonly IContactRepository _contacts;

        /// <summary>构造函数</summary>
        /// <param name="contacts">通讯录数据仓储</param>
        public ContactsConventionController(IContactRepository contacts)
        {
            _contacts = contacts;
        }

        /// <summary>
        /// 获取通讯录。
        /// </summary>
        /// <returns>通讯录清单。</returns>
        /// <remarks>
        /// 请求模式：
        /// GET /api/contactsconvention
        /// </remarks>
        // GET api/contactsconvention
        [HttpGet]
        public IEnumerable<Contact> Get()
        {
            return _contacts.GetAll();
        }

        /// <summary>
        /// 按 ID 获取某一通讯记录项。
        /// </summary>
        /// <param name="id">指定通讯记录项的唯一序列值</param>
        /// <returns>通讯记录项。</returns>
        /// <remarks>
        /// 请求模式：
        /// GET /api/contactsconvention/{guid}
        /// </remarks>
        // GET api/contactsconvention/{guid}
        [HttpGet("{id}", Name = "GetContactsConventionById")]
        public ActionResult<Contact> Get(string id)
        {
            var contact = _contacts.Get(id);

            if (contact == null)
            {
                return NotFound();
            }

            return Ok(contact);
        }

        /// <summary>
        /// 添加新的通讯录记录项。
        /// </summary>
        /// <param name="contact">通讯录记录项</param>
        /// <returns>包含新的通讯录记录项的创建执行结果。</returns>
        /// <remarks>
        /// 请求模式：
        /// POST /api/contactsconvention
        /// {
        ///   "firstName": #string#,
        ///   "lastName": #string#
        /// }
        /// </remarks>
        // POST api/contactsconvention
        [HttpPost]
        public IActionResult Post(Contact contact)
        {
            _contacts.Add(contact);

            return CreatedAtRoute("GetContactsConventionById", new { id = contact.ID }, contact);
        }

        #region snippet_ApiConventionMethod
        /// <summary>
        /// 更新现有通讯录记录项。
        /// </summary>
        /// <param name="id">指定通讯录记录项的唯一序列值</param>
        /// <param name="contact">通讯录记录项</param>
        /// <returns>处理更改的执行结果</returns>
        /// <remarks>
        /// 请求模式：
        /// PUT /api/contactsconvention/{guid}
        /// {
        ///   "id": #number#,
        ///   "firstName": #string#,
        ///   "lastName": #string#
        /// }
        /// </remarks>
        // PUT api/contactsconvention/{guid}
        [HttpPut("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public IActionResult Update(string id, Contact contact)
        {
            var contactToUpdate = _contacts.Get(id);

            if (contactToUpdate == null)
            {
                return NotFound();
            }

            _contacts.Update(contact);

            return NoContent();
        }
        #endregion

        /// <summary>
        /// 删除通讯录记录项。
        /// </summary>
        /// <param name="id">指定通讯录记录项的唯一序列值</param>
        /// <returns>处理删除的执行结果</returns>
        /// <remarks>
        /// 请求模式：
        /// DELETE /api/contactsconvention/{id}
        /// </remarks>
        /// <response code="204">没有内容。</response>
        /// <response code="404">没有相应的通讯录记录项</response>
        // DELETE api/contactsconvention/{guid}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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