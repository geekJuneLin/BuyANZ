using BuyANZCoupon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyANZCoupon.Dto
{
    public class CouponDto
    {
        public ApplicationUser User { get; set; }
        public string CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
