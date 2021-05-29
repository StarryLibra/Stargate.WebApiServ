using Microsoft.EntityFrameworkCore;
using Stargate.WebApiServ.Data.Models;

namespace Stargate.WebApiServ.Data
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
