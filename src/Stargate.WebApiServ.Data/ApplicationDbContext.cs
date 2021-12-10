global using System.ComponentModel.DataAnnotations;
global using Microsoft.EntityFrameworkCore;
global using Stargate.WebApiServ.Data.Models;

namespace Stargate.WebApiServ.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}
