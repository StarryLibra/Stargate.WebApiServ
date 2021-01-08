using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stargate.WebApiServ.Web.Controllers;

namespace Stargate.WebApiServ.Tests
{
    [TestClass]
    public class WeatherForecastUnitTests
    {
        private static WeatherForecastController _controller;
        private static List<string> _summaries;
        
        [ClassInitialize()]
        public static void WeatherForecastInitialize(TestContext testContext)
        {
            _controller = new WeatherForecastController(logger: null);
            _summaries = new List<string> { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
        }

        [TestMethod]
        [TestCategory("Sample")]
        public void TestGet()
        {

            var result = _controller.Get();
            Assert.IsNotNull(result);

            var enumerator = result.GetEnumerator();
            if (enumerator.MoveNext())
            {
                var forecast = enumerator.Current;
                Assert.IsTrue(_summaries.Contains(forecast.Summary), "无效的情况简报");
                Assert.IsTrue(forecast.TemperatureC >= -20 && forecast.TemperatureC <= 55, "摄氏温度超出范围");
                Assert.IsTrue(forecast.TemperatureF >= -4 && forecast.TemperatureF <= 131, "华氏温度超出范围");
                Assert.IsTrue(forecast.Date > DateTime.Now && forecast.Date < DateTime.Now.AddDays(5));
            }
            else
                Assert.Fail("无返回天气预报对象");
        }
    }
}
