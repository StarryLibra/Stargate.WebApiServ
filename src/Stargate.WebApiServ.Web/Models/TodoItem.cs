namespace Stargate.WebApiServ.Web.Models
{
    /// <summary>
    /// 示例用的待办事项数据模型
    /// </summary>
    public class TodoItem
    {
        /// <summary>唯一序列值</summary>
        public long Id { get; set; }

        /// <summary>名称</summary>
        public string Name { get; set; }

        /// <summary>是否完成</summary>
        public bool IsComplete { get; set; }

        /// <summary>保密内容</summary>
        public string Secret { get; set; }
    }
}
