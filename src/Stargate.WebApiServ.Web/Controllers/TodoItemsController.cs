using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stargate.WebApiServ.Web.Models;

namespace Stargate.WebApiServ.Web.Controllers
{
    /// <summary>
    /// 示例用的待办事项控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
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
        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            return await _context.TodoItems
                .Select(x => ItemToDTO(x))
                .ToListAsync();
        }

        /// <summary>
        /// 获取某一待办事项。
        /// </summary>
        /// <param name="id">指定待办事项的唯一序列值</param>
        /// <returns>包含表示待办事项的数据传输对象实例的执行结果。</returns>
        // GET: api/TodoItems/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        /// 更改待办事项。
        /// </summary>
        /// <param name="id">指定待办事项的唯一序列值</param>
        /// <param name="todoItemDTO">待办事项数据传输对象</param>
        /// <returns>处理更改的执行结果</returns>
        // PUT: api/TodoItems/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status404NotFound)]
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
        /// 创建新的待办事项。
        /// </summary>
        /// <param name="todoItemDTO">待办事项数据传输对象</param>
        /// <returns>包含表示待办事项的数据传输对象实例的创建执行结果。</returns>
        // POST: api/TodoItems
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<TodoItemDTO>> CreateTodoItem(TodoItemDTO todoItemDTO)
        {
            var todoItem = new TodoItem
            {
                IsComplete = todoItemDTO.IsComplete,
                Name = todoItemDTO.Name
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTodoItem),
                new { id = todoItem.Id },
                ItemToDTO(todoItem));
        }

        /// <summary>
        /// 删除待办事项。
        /// </summary>
        /// <param name="id">指定待办事项的唯一序列值</param>
        /// <returns>处理删除的执行结果</returns>
        // DELETE: api/TodoItems/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem is null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static TodoItemDTO ItemToDTO(TodoItem todoItem) => new TodoItemDTO
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete
        };

        private bool TodoItemExists(long id) => _context.TodoItems.Any(e => e.Id == id);

        #region These methods are just for testing populating the secret field

        /// <summary>
        /// 获取所有待办事项。
        /// </summary>
        /// <returns>所有待办事项(包含保密内容)的列表。</returns>
        // GET: api/TodoItems/test
        [HttpGet("test")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTestTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        /// <summary>
        /// 创建新的待办事项。
        /// </summary>
        /// <param name="todoItem">待办事项</param>
        /// <returns>包含表示待办事项实例的创建执行结果。</returns>
        // POST: api/TodoItems/test
        [HttpPost("test")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<TodoItem>> PostTestTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        #endregion
    }
}
