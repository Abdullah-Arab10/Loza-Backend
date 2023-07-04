global using Microsoft.EntityFrameworkCore;
using Loza.Entities;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Loza.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }


        public DbSet<Product> Product { get; set; }
        //public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        /* protected override void OnModelCreating(ModelBuilder modelBuilder)
         {

             var dateOnlyConverter = new ValueConverter<DateOnly, string>(
                 v => v.ToString("yyyy-MM-dd"),
                 v => DateOnly.Parse(v));
             modelBuilder.Entity<Order>()
                 .Property(u => u.Created_at)
                 .HasColumnType("nvarchar(10)")
                 .HasConversion(dateOnlyConverter);


             base.OnModelCreating(modelBuilder);
         }*/

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=Loza;Trusted_Connection=true;TrustServerCertificate=true;");
        }
    }
}
