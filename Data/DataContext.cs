global using Microsoft.EntityFrameworkCore;
using Loza.Entities;

namespace Loza.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)        
        {
            
        }
        
        public DbSet<Product>Product { get; set; }
        //public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<favorite> favorites { get; set; }
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=Loza;Trusted_Connection=true;TrustServerCertificate=true;");
        }
    }
}
