using Stargate.WebApiServ.Data;

// For more information on archive 'DbContext Lifetime, Configuration, and Initialization',
// visit https://docs.microsoft.com/zh-cn/ef/core/dbcontext-configuration/

namespace Stargate.WebApiServ.Web.Controllers
{
    /// <summary>
    /// ASP.NET Core 依赖关系注入中的 DbContext
    /// </summary>
    public class MyController
    {
        private readonly ApplicationDbContext _context;

        /// <summary>构造函数</summary>
        /// <param name="context">应用程序数据库上下文</param>
       public MyController(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
