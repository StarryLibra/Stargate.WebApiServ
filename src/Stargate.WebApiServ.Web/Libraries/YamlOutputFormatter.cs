// 参考微软文档《Custom formatters in ASP.NET Core Web API》，请访问：https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/custom-formatters?view=aspnetcore-6.0。
// 修改自文档《ASP.NET Core Custom Request/Response Formatters (YAML Formatters)》，
// 请访问：https://social.technet.microsoft.com/wiki/contents/articles/37764.asp-net-core-custom-requestresponse-formatters-yaml-formatters.aspx，
// 或者作者博客：https://fiyazhasan.me/dotnet-input-output-formatter

using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using YamlDotNet.Serialization;

namespace Stargate.WebApiServ.Web.Libraries;

/// <summary>
/// 用于 YAML 内容的 <see cref="TextOutputFormatter"/>。
/// </summary>
public class YamlOutputFormatter : TextOutputFormatter
{
    private readonly ISerializer _serializer;

    /// <summary>
    /// 初始化一个新的 <see cref="YamlOutputFormatter"/> 实例。
    /// </summary>
    /// <param name="serializer">YAML 串行化器</param>
    public YamlOutputFormatter(ISerializer serializer)
    {
        _serializer = serializer;

        this.SupportedMediaTypes.Add("application/x-yaml");
        this.SupportedMediaTypes.Add("application/*+x-yaml");
        this.SupportedMediaTypes.Add("application/yaml");
        this.SupportedMediaTypes.Add("text/x-yaml");
        this.SupportedMediaTypes.Add("text/yaml");

        this.SupportedEncodings.Add(Encoding.UTF8);
        this.SupportedEncodings.Add(Encoding.Unicode);
    }

    /// <inheritdoc />
    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (selectedEncoding == null)
            throw new ArgumentNullException(nameof(selectedEncoding));

        var response = context.HttpContext.Response;
        using var writer = context.WriterFactory(response.Body, selectedEncoding);
        _serializer.Serialize(writer, context.Object ?? new object());

        // Perf: call FlushAsync to call WriteAsync on the stream with any content left in the TextWriter's
        // buffers. This is better than just letting dispose handle it (which would result in a synchronous
        // write).
        await writer.FlushAsync();
    }
}
