using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyANZCoupon.Helpers
{
    public class PaginationList<T> : List<T>
    {
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;

        private PaginationList(int pageNumber, int pageSize, int totalCount, int totalPages, List<T> items)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            AddRange(items);
            TotalCount = totalCount;
            TotalPages = totalPages;
        }

        public static async Task<PaginationList<T>> CreatePaginationListAsync(int pageNumber, int pageSize, IQueryable<T> results)
        {
            var totalCount = await results.CountAsync();

            var TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            pageNumber = pageNumber > TotalPages ? (int)TotalPages : pageNumber;
            var numberToSkip = (pageNumber - 1) * pageSize;
            results = results.Skip(numberToSkip);
            results = results.Take(pageSize);

            return  new PaginationList<T>(pageNumber, pageSize, totalCount, TotalPages, await results.ToListAsync());
        }
    }
}
