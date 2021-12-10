namespace Stargate.WebApiServ.Data.Models;

public class Tag
{
    public int Id { get; set; }
    public string Text { get; set; } = String.Empty;
    public ICollection<Post> Posts { get; set; } = default!;
}
