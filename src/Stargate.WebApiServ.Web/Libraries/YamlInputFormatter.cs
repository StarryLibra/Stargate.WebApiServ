// 参考微软文档《Custom formatters in ASP.NET Core Web API》，请访问：https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/custom-formatters?view=aspnetcore-6.0。
// 修改自文档《ASP.NET Core Custom Request/Response Formatters (YAML Formatters)》，
// 请访问：https://social.technet.microsoft.com/wiki/contents/articles/37764.asp-net-core-custom-requestresponse-formatters-yaml-formatters.aspx，
// 或者作者博客：https://fiyazhasan.me/dotnet-input-output-formatter

using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using YamlDotNet.Serialization;

namespace Stargate.WebApiServ.Web.Libraries;

/// <summary>
/// 用于 YAML 内容的 <see cref="TextInputFormatter"/>。
/// </summary>
public class YamlInputFormatter : TextInputFormatter
{
    private readonly IDeserializer _deserializer;

    /// <summary>
    /// 初始化一个新的 <see cref="YamlInputFormatter"/> 实例。
    /// </summary>
    /// <param name="deserializer">YAML 反串行化器</param>
    public YamlInputFormatter(IDeserializer deserializer)
    {
        _deserializer = deserializer;

        this.SupportedMediaTypes.Add("application/x-yaml");
        this.SupportedMediaTypes.Add("application/*+x-yaml");
        this.SupportedMediaTypes.Add("application/yaml");
        this.SupportedMediaTypes.Add("text/x-yaml");
        this.SupportedMediaTypes.Add("text/yaml");

        this.SupportedEncodings.Add(UTF8EncodingWithoutBOM);
        this.SupportedEncodings.Add(UTF16EncodingLittleEndian);
    }

    /// <inheritdoc />
    public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (encoding == null)
            throw new ArgumentNullException(nameof(encoding));

        var request = context.HttpContext.Request;

        using var streamReader = context.ReaderFactory(request.Body, encoding);
        var type = context.ModelType;
        try
        {
            var model = _deserializer.Deserialize(streamReader, type);
            return InputFormatterResult.SuccessAsync(model);
        }
        catch (Exception)
        {
            return InputFormatterResult.FailureAsync();
        }
    }
}
