using System;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stargate.WebApiServ.Web.Controllers;

namespace Stargate.WebApiServ.Tests
{
    [TestClass]
    public class WeatherForecastUnitTests
    {
        private static WeatherForecastController _controller;
        private static readonly string[] _summaries = { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
        
        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            _controller = new WeatherForecastController(logger: NullLogger<WeatherForecastController>.Instance);
        }

        [TestMethod]
        [TestCategory("WeatherForecast")]
        public void TestGet()
        {
            var result = _controller.Get();
            Assert.IsNotNull(result);

            foreach (var forecast in result)
            {
                Assert.IsTrue(_summaries.Contains(forecast.Summary), "无效的情况简报");
                Assert.IsTrue(forecast.TemperatureC >= -20 && forecast.TemperatureC <= 55, "摄氏温度超出范围");
                Assert.IsTrue(forecast.TemperatureF >= -4 && forecast.TemperatureF <= 131, "华氏温度超出范围");
                Assert.IsTrue(forecast.Date > DateTime.Now && forecast.Date < DateTime.Now.AddDays(5));
            }
        }
    }
}
