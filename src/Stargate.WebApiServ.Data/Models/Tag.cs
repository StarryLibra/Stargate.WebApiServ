using System.Collections.Generic;

namespace Stargate.WebApiServ.Data.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
