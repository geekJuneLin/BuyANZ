using BuyANZCoupon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyANZCoupon.Dto
{
    public class CouponCreateDto
    {
        public string? CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public string CodeType { get; set; }
    }
}
