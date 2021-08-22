using BuyANZCoupon.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BuyANZCoupon.Dto
{
    public class CouponUpdateDto
    {
        public string CouponCode { get; set; }
        public ApplicationUser? User { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountAmount { get; set; }
        public CodeType CodeType { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
