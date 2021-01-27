using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stargate.WebApiServ.Web.Controllers;

namespace Stargate.WebApiServ.Tests
{
    [TestClass]
    public class ValuesUnitTests
    {
        private static ValuesController _controller;

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            _controller = new ValuesController();
        }

        [TestMethod]
        [TestCategory("Values")]
        public void TestGet()
        {
            var result = _controller.Get();
            Assert.AreEqual(2, result.Count(), message:"结果中的字符串数量不正确");

            var enumerator = result.GetEnumerator();
            enumerator.MoveNext();
            Assert.AreEqual("value1", enumerator.Current, message:"结果的第一个字符串值不一致");
            enumerator.MoveNext();
            Assert.AreEqual("value2", enumerator.Current, message:"结果的第二个字符串值不一致");
        }

        [TestMethod]
        [TestCategory("Values")]
        public void TestGetById()
        {
            var result = _controller.Get(5);
            Assert.AreEqual("value", result, message:"结果字符串值不正确");
        }
    }
}
