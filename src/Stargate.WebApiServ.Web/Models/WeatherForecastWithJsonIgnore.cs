using System.Text.Json.Serialization;

// For more information on archive 'How to ignore properties with System.Text.Json',
// visit https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-ignore-properties?pivots=dotnet-6-0

namespace Stargate.WebApiServ.Web.Models;

/// <summary>
/// 模拟天气预报的数据模型（带指定忽略某些特性的属性来生成JSON）
/// </summary>
public class WeatherForecastWithJsonIgnore
{
    /// <summary>天气预报时间</summary>
    public DateTimeOffset Date { get; set; }

    /// <summary>摄氏温度</summary>
    public int TemperatureCelsius { get; set; }

    /// <summary>天气摘要</summary>
    [JsonIgnore]
    public string? Summary { get; set; }

    /// <summary>风速</summary>
    public int WindSpeedReadOnly { get; private set; } = 35;
}
