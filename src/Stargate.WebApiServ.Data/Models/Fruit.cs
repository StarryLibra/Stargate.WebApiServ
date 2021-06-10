using System.ComponentModel.DataAnnotations;

namespace Stargate.WebApiServ.Data.Models
{
    public class Fruit
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
