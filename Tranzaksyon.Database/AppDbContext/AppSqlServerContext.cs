using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tranzaksyon.Database.Models;

namespace Tranzaksyon.Database.AppDbContext
{
    public class AppSqlServerContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Item> Items { get; set; }

        public DbSet<OrderLog> OrderLogs { get; set; }

        public AppSqlServerContext(DbContextOptions options) : base(options)
        {
        }
    }
}
