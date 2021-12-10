using Microsoft.EntityFrameworkCore;

namespace Stargate.WebApiServ.Web.Controllers;

/// <summary>
/// 示例用的水果控制器
/// </summary>
[Produces("application/json")]
[Route("[controller]")]
public class FruitsController : ControllerBase
{
    private readonly SampleContext _context;

    /// <summary>构造函数</summary>
    /// <param name="context">水果数据仓储</param>
    public FruitsController(SampleContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取水果列表。
    /// </summary>
    /// <returns>水果列表。</returns>
    [HttpGet]
    public ActionResult<IEnumerable<Fruit>> Get() =>
        _context.Fruits.ToList();

    /// <summary>
    /// 根据 ID 获取水果
    /// </summary>
    /// <param name="id">水果唯一标识符</param>
    /// <returns>相应的水果</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Fruit>> GetById(int id)
    {
        var fruit = await _context.Fruits.FindAsync(id);

        if (fruit == null)
        {
            return NotFound();
        }

        return fruit;
    }

    /// <summary>
    /// 新添水果
    /// </summary>
    /// <param name="fruit">新水果</param>
    /// <returns>新增后的水果</returns>
    [HttpPost]
    public async Task<ActionResult<Fruit>> Create([FromBody] Fruit fruit)
    {
        _context.Fruits.Add(fruit);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = fruit.Id }, fruit);
    }

    /// <summary>
    /// 更新现有水果。
    /// </summary>
    /// <param name="id">指定水果的唯一序列值</param>
    /// <param name="fruit">水果对象</param>
    /// <returns>处理更改的执行结果</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Fruit fruit)
    {
        if (id != fruit.Id)
        {
            return BadRequest();
        }

        _context.Entry(fruit).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// 删除水果。
    /// </summary>
    /// <param name="id">指定水果的唯一序列值</param>
    /// <returns>处理删除的执行结果</returns>
    /// <response code="204">空白内容</response>
    /// <response code="404">没有相应的水果</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Delete(int id)
    {
        var fruit = await _context.Fruits.FindAsync(id);

        if (fruit == null)
        {
            return NotFound();
        }

        _context.Fruits.Remove(fruit);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
