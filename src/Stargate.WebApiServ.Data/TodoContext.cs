using Microsoft.EntityFrameworkCore;
using Stargate.WebApiServ.Data.Models;

namespace Stargate.WebApiServ.Data
{
    /// <summary>
    /// 待办事项数据库上下文
    /// </summary>
    public class TodoContext : DbContext
    {
        /// <summary>构造函数</summary>
        /// <param name="options">待办事项数据库上下文的配置选项</param>
        public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }

        /// <summary>待办事项数据集</summary>
        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
