using System;

namespace Stargate.WebApiServ.Web.Models
{
    /// <summary>
    /// 创建.NET项目时自带的示例数据模型（模拟天气预报模型）
    /// </summary>
    public class WeatherForecast
    {
        /// <summary>天气预报日期</summary>
        public DateTime Date { get; set; }

        /// <summary>天气温度（按摄氏温标）</summary>
        public int TemperatureC { get; set; }

        /// <summary>天气温度（按华氏温标）</summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary>天气摘要</summary>
        public string Summary { get; set; }
    }
}
