using System;
using Microsoft.EntityFrameworkCore;
using Stargate.WebApiServ.Data.Models;

namespace Stargate.WebApiServ.Data
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=Blogging;Integrated Security=True");
            optionsBuilder.LogTo(Console.WriteLine);
        }
    }
}
