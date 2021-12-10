namespace Stargate.WebApiServ.Web.Models;

/// <summary>
/// 作者
/// </summary>
public class Author
{
    /// <summary>
    /// 姓名
    /// </summary>
    /// <value>表达作者姓名的字符串值。</value>
    public string Name { get; set; } = String.Empty;
    /// <summary>
    /// 推特
    /// </summary>
    /// <value>表示作者推特账号的字符串值。</value>
    public string Twitter { get; set; } = String.Empty;
}
