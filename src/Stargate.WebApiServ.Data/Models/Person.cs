using System.ComponentModel.DataAnnotations;

namespace Stargate.WebApiServ.Data.Models
{
    public class Person
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
