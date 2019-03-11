using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.EntityFrameworkCore;

namespace DScrib2.Models
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=DScrib2;Trusted_Connection=True;");
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}