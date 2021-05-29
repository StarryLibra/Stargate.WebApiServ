using System.ComponentModel.DataAnnotations;

namespace Stargate.WebApiServ.Data.Models
{
    #region snippet_ProductClass
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public bool IsOnSale { get; set; }
    }
    #endregion
}
