namespace Stargate.WebApiServ.Data.Models;

#region snippet_ProductClass
public class Product
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = String.Empty;

    [Required]
    public string Description { get; set; } = String.Empty;

    public bool IsOnSale { get; set; }
}
#endregion
