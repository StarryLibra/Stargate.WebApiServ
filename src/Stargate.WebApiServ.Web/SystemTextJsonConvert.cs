using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Stargate.WebApiServ.Web.SystemTextJsonConvert
{
    /// <summary>
    /// <c>DateTime</c> 结构的 JSON 序列化类自定义转换器
    /// </summary>
    public class DateTimeMyConverter : JsonConverter<DateTime>
    {
        /// <summary>日期格式</summary>
        public string DateTimeFormat { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dateTimeFormat">JSON 序列化时的日期格式</param>
        public DateTimeMyConverter(string dateTimeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            DateTimeFormat = dateTimeFormat;
        }

        /// <inheritdoc/>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateTime.Parse(reader.GetString());

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(DateTimeFormat));
    }


    /// <summary>
    /// <c>DateTime?</c> 可空类型的 JSON 序列化类自定义转换器
    /// </summary>
    public class DateTimeNullableMyConverter : JsonConverter<DateTime?>
    {
        /// <summary>日期格式</summary>
        public string DateTimeFormat { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dateTimeFormat">JSON 序列化时的日期格式</param>
        public DateTimeNullableMyConverter(string dateTimeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            DateTimeFormat = dateTimeFormat;
        }

        /// <inheritdoc/>
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => String.IsNullOrEmpty(reader.GetString()) ? default(DateTime?) : DateTime.Parse(reader.GetString());

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
            => writer.WriteStringValue(value?.ToString(DateTimeFormat));
    }
}
