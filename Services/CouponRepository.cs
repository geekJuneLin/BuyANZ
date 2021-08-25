using BuyANZCoupon.Database;
using BuyANZCoupon.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyANZCoupon.Services
{
    public class CouponRepository : ICouponRepository
    {
        private readonly AppDbContext _dbContext;

        public CouponRepository(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<Coupon> GetCouponById(string couponId)
        {
            var coupon = await _dbContext.Coupons.FirstOrDefaultAsync(c => c.CouponId.Equals(couponId));
            return coupon;
        }

        public void AddCoupon(Coupon coupon)
        {
            _dbContext.Coupons.Add(coupon);
        }

        public async Task<bool> IsCouonExists(string couponId)
        {
            return await _dbContext.Coupons.AnyAsync(c => c.CouponId == couponId);
        }

        public async Task<bool> IsUserExists(string userId)
        {
            return await _dbContext.Users.AnyAsync(u => u.Id == userId);
        }

        public async Task<IList<Coupon>> GetAllValidCouponsAsync(string userId)
        {
            return await _dbContext.Coupons.Where(c => c.User.Id == userId).ToListAsync();
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> SaveAsync()
        {
            return (await _dbContext.SaveChangesAsync() >= 0);
        }

        public string RandomCouponCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
    }
}
