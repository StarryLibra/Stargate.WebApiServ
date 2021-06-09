using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stargate.WebApiServ.Web.Models;

namespace Stargate.WebApiServ.Web.Controllers
{
    /// <summary>
    /// 创建 .NET 项目时自带的示例控制器（模拟天气预报控制器）
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        /// <summary>构造函数</summary>
        /// <param name="logger">与模拟天气预报控制器关联的应用日志</param>
        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 创建 .NET 项目时自带的示例方法（模拟获取天气预报）。
        /// </summary>
        /// <returns>天气预报。</returns>
        /// <response code="200">成功获取天气预报</response>
        [HttpGet]
        [ProducesResponseType(typeof(WeatherForecast[]), StatusCodes.Status200OK)]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// 处理 ASP.NET Core Web API 中的错误。
        /// </summary>
        /// <param name="city">指定城市</param>
        /// <returns>某城市的天气预报。</returns>
        /// <exception cref="System.ArgumentException">当城市不对时抛出异常</exception>
        [HttpGet("{city}")]
        public WeatherForecast Get(string city)
        {
            if (!string.Equals(city?.TrimEnd(), "Redmond", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    $"We don't offer a weather forecast for {city}.", nameof(city));
            }

            return GetWeather().First();
        }

        private IEnumerable<WeatherForecast> GetWeather() => Get();

        #region snippet
        /// <summary>
        /// 模拟获取天气预报出错。
        /// </summary>
        /// <returns>出错文字提示。</returns>
        [HttpGet("error")]
        public IActionResult GetError()
        {
            return Problem("Something went wrong!");
        }
        #endregion
    }
}
