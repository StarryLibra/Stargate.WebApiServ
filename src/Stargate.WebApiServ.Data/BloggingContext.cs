namespace Stargate.WebApiServ.Data;

public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; } = null!;
    public DbSet<Post> Posts { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=(localdb)\mssqllocaldb;Database=Stargate;Integrated Security=True");
        optionsBuilder.LogTo(Console.WriteLine);
    }
}
