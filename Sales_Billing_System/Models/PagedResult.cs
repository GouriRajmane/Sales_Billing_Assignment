using System;
using System.Collections.Generic;

namespace Sales_Billing_System.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages
        {
            get
            {
                if (PageSize <= 0)
                    return 0;

                return (int)Math.Ceiling(
                    (double)TotalRecords / PageSize
                );
            }
        }

        public PagedResult()
        {
            Items = new List<T>();
        }
    }
}