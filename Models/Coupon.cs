using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuyANZCoupon.Models
{
    public class Coupon
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CouponId { get; set; }
        [Required]
        public string CouponCode { get; set; }
        public ApplicationUser? User { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Required]
        public decimal DiscountAmount { get; set; }
        [Required]
        public CodeType CodeType { get; set; }
        [Required]
        public DateTime ExpirationDate { get; set; }
    }

    public enum CodeType
    {
        Fixed,
        Random
    }
}
