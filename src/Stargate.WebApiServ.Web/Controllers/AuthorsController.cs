using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Stargate.WebApiServ.Web.Models;

#pragma warning disable API1000
namespace Stargate.WebApiServ.Web.Controllers
{
    /// <summary>
    /// 示例用的作者控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : Controller
    {
        private readonly Authors _authors;

        /// <summary>构造函数</summary>
        public AuthorsController()
        {
            _authors = new Authors();
        }

        #region snippet_get
        /// <summary>
        /// 获取作者清单。
        /// </summary>
        /// <returns>作者清单。</returns>
        /// <remarks>
        /// 请求模式：
        /// GET /api/authors
        /// </remarks>
        // GET: api/authors
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(_authors.List());
        }
        #endregion

        #region snippet_search
        /// <summary>
        /// 通过姓名的片段部分搜索作者。
        /// </summary>
        /// <returns>获取作者的结果。</returns>
        /// <remarks>
        /// 请求模式：
        /// GET /api/authors/search?namelike={namelike}
        /// </remarks>
         // GET: api/authors/search?namelike=th
        [HttpGet("Search")]
        public IActionResult Search(string namelike)
        {
            var result = _authors.GetByNameSubstring(namelike);
            if (!result.Any())
            {
                return NotFound(namelike);
            }
            return Ok(result);
        }
        #endregion

        #region snippet_alias
        /// <summary>
        /// 通过别名获取作者。
        /// </summary>
        /// <returns>获取得到的作者对象。</returns>
        /// <remarks>
        /// 请求模式：
        /// GET /api/authors/{alias}
        /// </remarks>
        // GET api/authors/RickAndMSFT
        [HttpGet("{alias}")]
        public Author Get(string alias)
        {
            return _authors.GetByAlias(alias);
        }
        #endregion

        #region snippet_about
        /// <summary>
        /// 获取本控制器的“关于”信息。
        /// </summary>
        /// <returns>“关于”信息内容。</returns>
        /// <remarks>
        /// 请求模式：
        /// GET /api/authors/about
        /// </remarks>
        // GET api/authors/about
        [HttpGet("About")]
        public ContentResult About()
        {
            return Content("An API listing authors of docs.asp.net.");
        }
        #endregion

        #region snippet_string
        /// <summary>
        /// 获取本控制器的版本信息。
        /// </summary>
        /// <returns>版本信息。</returns>
        /// <remarks>
        /// 请求模式：
        /// GET /api/authors/version
        /// </remarks>
        // GET api/authors/version
        [HttpGet("version")]
        public string Version()
        {
            return "Version 1.0.0";
        }
        #endregion

    }


    /// <summary>
    /// 作者(们)
    /// </summary>
    public class Authors 
    {
        /// <summary>
        /// 获取作者清单。
        /// </summary>
        /// <returns>包含所有作者对象的列表对象。</returns>
        public List<Author> List()
        {
            return new List<Author>
            {
                new Author {Name="Steve Smith", Twitter="ardalis"},
                new Author {Name="Rick Anderson", Twitter="RickAndMSFT"},
                new Author {Name="Rachel Appel", Twitter="rachelappel"},
                new Author {Name="Daniel Roth", Twitter="danroth27"}
            }
            .OrderBy(a => a.Name).ToList();
        }

        /// <summary>
        /// 通过推特账号获取相应作者。
        /// </summary>
        /// <param name="twitterAlias">推特别名</param>
        /// <returns>与推特账号相对应的作者对象。</returns>
        public Author GetByAlias(string twitterAlias)
        {
            string loweredAlias = twitterAlias.ToLowerInvariant();
            return List()
                .FirstOrDefault(a => a.Twitter.ToLowerInvariant() == loweredAlias);
        }

        /// <summary>
        /// 通过姓名的片段部分获取作者。
        /// </summary>
        /// <param name="nameSubstring">姓名的片段部分</param>
        /// <returns>与姓名的片段部分相对应的作者对象。</returns>
        public List<Author> GetByNameSubstring(string nameSubstring)
        {
            return List()
                .Where(a =>
                    a.Name.IndexOf(nameSubstring, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                        .ToList();
        }
    }
}
