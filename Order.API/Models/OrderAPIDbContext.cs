using System;
using Microsoft.EntityFrameworkCore;
using Order.API.Models.Entites;

namespace Order.API.Models
{
	public class OrderAPIDbContext : DbContext
	{
        public OrderAPIDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Entites.Order> Orders { get; set; }
    }
}

