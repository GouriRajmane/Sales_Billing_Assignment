using System.Collections.Generic;

namespace Sales_Billing_System.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public bool HasMore { get; set; }                       // more rows exist after this batch
        public Dictionary<int, int> GroupCounts { get; set; }   // Product list: categoryId -> product count

        public PagedResult()
        {
            Items = new List<T>();
            GroupCounts = new Dictionary<int, int>();
        }
    }
}