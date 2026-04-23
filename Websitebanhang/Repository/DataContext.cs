using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Websitebanhang.Models;

namespace Websitebanhang.Repository
{
    public class DataContext : IdentityDbContext<AppUserModel>
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<OrderDetails>().Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Entity<ProductModel>().Property(p => p.Price).HasColumnType("decimal(18,2)");
        }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<BrandModel> Brands { get; set; }
        
        public DbSet<CategoryModel> Categories { get; set; }

        public DbSet<ProductModel> Products { get; set; }

        public DbSet<OrderModel> Orders { get; set; }

        public DbSet<OrderDetails> OrderDetails { get; set; }

        public DbSet<RaitingModel> Raitings { get; set; }
    }
}
