using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stargate.WebApiServ.Data;
using Stargate.WebApiServ.Web.Controllers;

namespace Stargate.WebApiServ.Tests
{
    [TestClass]
    public class TodoItemsUnitTests
    {
        private static TodoItemsController _controller;
        private static TodoContext _dbcontext;
        
        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            _dbcontext = new TodoContext(
                new DbContextOptionsBuilder<TodoContext>()
                    .UseInMemoryDatabase("TodoList")
                    .Options
            );
            _controller = new TodoItemsController(_dbcontext);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _dbcontext.Dispose();
        }

        [TestMethod]
        [TestCategory("TodoItems")]
        public void TestGet()
        {
            var actionResult = _controller.GetTodoItems().Result;
            Assert.IsNotNull(actionResult);
            var result = actionResult.Value;
            Assert.IsNotNull(result);

            foreach (var item in result)
            {
                Assert.IsTrue(item.Id > 0, "待办事项唯一序列值必须是大于零的长整数");
            }
        }
    }
}
