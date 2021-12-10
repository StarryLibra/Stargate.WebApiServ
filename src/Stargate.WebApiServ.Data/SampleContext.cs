namespace Stargate.WebApiServ.Data;

public class SampleContext : DbContext
{
    public SampleContext(DbContextOptions<SampleContext> options)
        : base(options)
    {
    }

    public DbSet<Fruit> Fruits { get; set; } = null!;
    public DbSet<Person> People { get; set; } = null!;
}
