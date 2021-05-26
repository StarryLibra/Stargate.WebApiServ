using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stargate.WebApiServ.Web.Models;

// For more information on archive 'Tutorial: Create a web API with ASP.NET Core',
// visit https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-5.0&tabs=visual-studio

namespace Stargate.WebApiServ.Web.Controllers
{
    /// <summary>
    /// 示例用的待办事项控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        /// <summary>构造函数</summary>
        /// <param name="context">待办事项数据库上下文</param>
        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有待办事项。
        /// </summary>
        /// <returns>所有待办事项的列表。</returns>
        /// <remarks>
        /// 请求模式：
        /// GET /api/TodoItems
        /// </remarks>
        /// <response code="200">成功获取待办事项列表</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            return await _context.TodoItems
                .Select(x => ItemToDTO(x))
                .ToListAsync();
        }

        /// <summary>
        /// 按 ID 获取项。
        /// </summary>
        /// <param name="id">指定待办事项的唯一序列值</param>
        /// <returns>包含表示待办事项的数据传输对象实例的列表。</returns>
        /// <remarks>
        /// 请求模式：
        /// GET /api/TodoItems/{id}
        /// </remarks>
        /// <response code="200">成功获取指定的待办事项</response>
        /// <response code="404">没有相应的待办事项</response>
        // GET: api/TodoItems/1
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem is null)
            {
                return NotFound();
            }

            return ItemToDTO(todoItem);
        }

        /// <summary>
        /// 更新现有项。
        /// </summary>
        /// <param name="id">指定待办事项的唯一序列值</param>
        /// <param name="todoItemDTO">待办事项数据传输对象</param>
        /// <returns>处理更改的执行结果</returns>
        /// <remarks>
        /// 请求模式：
        /// PUT /api/TodoItems/{id}
        /// {
        ///   "id": #number#,
        ///   "name":  #string#,
        ///   "isComplete": #boolean#
        /// }
        /// </remarks>
        /// <response code="204">成功更改指定的待办事项</response>
        /// <response code="400">指定的待办事项唯一序列值与请求内容中的 id 值不一致</response>
        /// <response code="404">没有相应的待办事项</response>
        // PUT: api/TodoItems/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
        {
            if (id != todoItemDTO.Id)
            {
                return BadRequest();
            }

            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem is null)
            {
                return NotFound();
            }

            todoItem.Name = todoItemDTO.Name;
            todoItem.IsComplete = todoItemDTO.IsComplete;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// 添加新项。
        /// </summary>
        /// <param name="todoItemDTO">待办事项数据传输对象</param>
        /// <returns>包含表示待办事项的数据传输对象实例的创建执行结果。</returns>
        /// <remarks>
        /// 请求模式：
        /// POST /api/TodoItems
        /// {
        ///   "name":  #string#,
        ///   "isComplete": #boolean#
        /// }
        /// </remarks>
        /// <response code="201">成功创建待办事项</response>
        /// <response code="400">无效的请求</response>
        // POST: api/TodoItems
        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> CreateTodoItem(TodoItemDTO todoItemDTO)
        {
            var todoItem = new TodoItem
            {
                IsComplete = todoItemDTO.IsComplete,
                Name = todoItemDTO.Name
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, ItemToDTO(todoItem));
        }

        /// <summary>
        /// 删除项。
        /// </summary>
        /// <param name="id">指定待办事项的唯一序列值</param>
        /// <returns>处理删除的执行结果</returns>
        /// <remarks>
        /// 请求模式：
        /// DELETE /api/TodoItems/{id}
        /// </remarks>
        /// <response code="200">成功删除指定的待办事项</response>
        /// <response code="400">无效的请求</response>
        /// <response code="404">没有相应的待办事项</response>
        // DELETE: api/TodoItems/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem is null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool TodoItemExists(long id) =>
            _context.TodoItems.Any(e => e.Id == id);

        private static TodoItemDTO ItemToDTO(TodoItem todoItem) => new TodoItemDTO
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete
        };

        #region These methods are just for testing populating the secret field

        /// <summary>
        /// 按 ID 获取项(包含保密内容)。
        /// </summary>
        /// <param name="id">指定待办事项的唯一序列值</param>
        /// <returns>包含表示待办事项(包含保密内容)的列表。</returns>
        /// <remarks>
        /// 请求模式：
        /// GET /api/TodoItems/{id}/over-posting
        /// </remarks>
        /// <response code="404">没有相应的待办事项</response>
        // GET: api/TodoItems/1/over-posting
        [HttpGet("{id}/over-posting")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<TodoItem>> GetTodoItemWithOverposting(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem is null)
            {
                return NotFound();
            }

            return todoItem;
        }

        /// <summary>
        /// 更新现有项(包含保密内容)。
        /// </summary>
        /// <param name="id">指定待办事项的唯一序列值</param>
        /// <param name="todoItem">待办事项(包含保密内容)</param>
        /// <returns>处理更改的执行结果</returns>
        /// <remarks>
        /// 请求模式：
        /// PUT /api/TodoItems/{id}/over-posting
        /// {
        ///   "id": #number#,    
        ///   "name":  #string#,
        ///   "isComplete": #boolean#,
        ///   "secret": #string#
        /// }
        /// </remarks>
        /// <response code="400">指定的待办事项唯一序列值与请求内容中的 id 值不一致</response>
        /// <response code="404">没有相应的待办事项</response>
        // PUT: api/TodoItems/1/over-posting
        [HttpPut("{id}/over-posting")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// 添加新项(包含保密内容)。
        /// </summary>
        /// <param name="todoItem">待办事项(包含保密内容)</param>
        /// <returns>包含表示待办事项(包含保密内容)实例的创建执行结果。</returns>
        /// <remarks>
        /// 请求模式：
        /// POST /api/TodoItems/over-posting
        /// {
        ///   "name":  #string#,
        ///   "isComplete": #boolean#,
        ///   "secret": #string#
        /// }
        /// </remarks>
        // POST: api/TodoItems/over-posting
        [HttpPost("over-posting")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        #endregion
    }
}
