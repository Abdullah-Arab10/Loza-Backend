using Loza.Entities;
using Loza.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection.Emit;

namespace Loza.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<User> users { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Ignore<IdentityUserLogin<string>>();
            builder.Ignore<IdentityUserRole<string>>();
            builder.Ignore<IdentityUserClaim<string>>();
            builder.Ignore<IdentityUserToken<string>>();
            builder.Ignore<IdentityUser<string>>();
            builder.Ignore<User>();

            builder.Entity<User>().Ignore(x => x.TwoFactorEnabled)
                                  .Ignore(x => x.LockoutEnd)
                                  .Ignore(x => x.LockoutEnabled)
                                  .Ignore(x => x.AccessFailedCount)
                                  .Ignore(x => x.PhoneNumberConfirmed)
                                  .Ignore(x => x.UserName)
                                  .Ignore(x => x.SecurityStamp)
                                  .Ignore(x => x.ConcurrencyStamp)
                                  .Ignore(x => x.EmailConfirmed);

            var dateOnlyConverter = new ValueConverter<DateOnly, string>(
                v => v.ToString("yyyy-MM-dd"),
                v => DateOnly.Parse(v));
            builder.Entity<User>()
                .Property(u => u.DateOfBirth)
                .HasColumnType("nvarchar(10)")
                .HasConversion(dateOnlyConverter);

            base.OnModelCreating(builder);

        }

    }
}
