using System;

namespace Stargate.WebApiServ.Web.Models
{
    /// <summary>
    /// 创建.NET项目时自带的示例数据模型（模拟天气预报模型）
    /// </summary>
    public class WeatherForecast
    {
        /// <summary>
        /// 天气预报日期
        /// </summary>
        /// <value>日期</value>
        public DateTime Date { get; set; }

        /// <summary>
        /// 天气温度（按摄氏温标）
        /// </summary>
        /// <value>摄氏温标温度</value>
        public int TemperatureC { get; set; }

        /// <summary>
        /// 天气温度（按华氏温标）
        /// </summary>
        /// <returns>华氏温标温度</returns>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary>
        /// 天气摘要
        /// </summary>
        /// <value>对天气情况的简要描述</value>
        public string Summary { get; set; }
    }
}
