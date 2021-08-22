using BuyANZCoupon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BuyANZCoupon.Database
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Init the seed data of UserRole and AdminUser
            modelBuilder.Entity<ApplicationUser>(u =>
            {
                u.HasMany(x => x.UserRoles)
                .WithOne()
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            });

            // Init the admin role seed data
            var adminRoleId = Guid.NewGuid().ToString();
            modelBuilder.Entity<IdentityRole>().HasData(
                    new IdentityRole
                    {
                        Id = adminRoleId,
                        Name = "Admin",
                        NormalizedName = "Admin".ToUpper()
                    }
                );

            // Init seed data of the normal user
            var seedUser = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "test@test.com",
                NormalizedUserName = "test@test.com".ToUpper(),
                Email = "test@test.com",
                NormalizedEmail = "test@test.com".ToUpper(),
                TwoFactorEnabled = false,
                EmailConfirmed = true,
                Coupons = new Collection<Coupon>() { }
            };
            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
            seedUser.PasswordHash = passwordHasher.HashPassword(seedUser, "test123.");
            modelBuilder.Entity<ApplicationUser>().HasData(seedUser);

            var adminUserId = Guid.NewGuid().ToString();
            var adminUser = new ApplicationUser()
            {
                Id = adminUserId,
                UserName = "admin@buyAnz.com",
                NormalizedUserName = "admin@buyAnz.com".ToUpper(),
                Email = "admin@buyAnz.com",
                NormalizedEmail = "admin@buyAnz.com".ToUpper(),
                TwoFactorEnabled = false,
                EmailConfirmed = true,
            };
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "admin123.");
            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasData(
                    new IdentityUserRole<string>
                    {
                        RoleId = adminRoleId,
                        UserId = adminUserId
                    }
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
