namespace Stargate.WebApiServ.Data.Models;

public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; } = String.Empty;
    public string? Content { get; set; }

    public int BlogId { get; set; }
    public Blog Blog { get; set; } = new();
}
