using Microsoft.EntityFrameworkCore;
using Mapster;
using Stargate.WebApiServ.Web.Models;

// For more information on archive 'Tutorial: Create a web API with ASP.NET Core',
// visit https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-6.0&tabs=visual-studio

#pragma warning disable API1000
namespace Stargate.WebApiServ.Web.Controllers;

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
            .ProjectToType<TodoItemDTO>()
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
        if (todoItem == null)
        {
            return NotFound();
        }

        return todoItem.Adapt<TodoItemDTO>();
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
    ///   "name": #string#,
    ///   "isComplete": #boolean#
    /// }
    /// </remarks>
    /// <response code="204">成功更改指定的待办事项</response>
    /// <response code="400">指定的待办事项唯一序列值与请求内容中的 id 值不一致</response>
    /// <response code="404">没有相应的待办事项</response>
    // PUT: api/TodoItems/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
    {
        if (id != todoItemDTO.Id)
        {
            return BadRequest();
        }

        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null)
        {
            return NotFound();
        }

        todoItemDTO.Adapt(todoItem);

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
    ///   "name": #string#,
    ///   "isComplete": #boolean#
    /// }
    /// </remarks>
    /// <response code="201">成功创建待办事项</response>
    /// <response code="400">无效的请求</response>
    // POST: api/TodoItems
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TodoItemDTO>> CreateTodoItem(TodoItemDTO todoItemDTO)
    {
        var todoItem = todoItemDTO.Adapt<TodoItem>();

        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetTodoItem),
            new { id = todoItem.Id },
            todoItem.Adapt<TodoItemDTO>());
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
    /// <response code="204">成功删除指定的待办事项</response>
    /// <response code="400">无效的请求</response>
    /// <response code="404">没有相应的待办事项</response>
    // DELETE: api/TodoItems/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(long id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null)
        {
            return NotFound();
        }

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TodoItemExists(long id) => _context.TodoItems.Any(e => e.Id == id);

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
