using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BuyANZCoupon.Helpers
{
    public class PaginationParameters
    {
        private int _PageSize = 5;
        public int PageSize {
            get
            {
                return _PageSize;
            }
            set {
                if (value >= 1)
                {
                    _PageSize = value;
                }
            } 
        }
        private int _PageNumber = 1;
        const int maxPageSize = 50;
        public int PageNumber {
            get
            {
                return _PageNumber;
            }
            set {
                if (value > 1)
                {
                    _PageNumber = (value > maxPageSize) ? maxPageSize : value;
                }
            } 
        }
    }
}
