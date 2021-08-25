using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BuyANZCoupon.Helpers
{
    public class CouponFilterParameters
    {
        public string Price {
            get
            {
                return _Price;
            }
            set {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Regex regex = new Regex(@"([a-zA-Z]+)(\d+\.?\d*)");
                    Match match = regex.Match(value);

                    if (match.Success)
                    {
                        Operator = match.Groups[1].Value;
                        FilterValue = decimal.Parse(match.Groups[2].Value);
                    }
                }
                _Price = value;
            } 
        }
        private string _Price { get; set; }
        public string Operator { get; set; }
        public decimal? FilterValue { get; set; }
    }
}
