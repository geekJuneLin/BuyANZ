using BuyANZCoupon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyANZCoupon.Services
{
    public interface ICouponRepository
    {
        public Task<IList<Coupon>> GetAllValidCouponsAsync(string userId);
        public void AddCoupon(Coupon coupon);
        public Task<Coupon> GetCouponById(string couponId);
        public Task<bool> IsCouonExists(string couponId);
        public Task<bool> IsUserExists(string userId);
        public Task<bool> SaveAsync();
        public string RandomCouponCode();

        public Task<ApplicationUser> GetUserById(string userId);
    }
}
