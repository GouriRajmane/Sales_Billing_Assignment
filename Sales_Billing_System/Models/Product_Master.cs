using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sales_Billing_System.Models
{
    public class Product_Master
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(100)]
        public string ProductName { get; set; }

        [StringLength(50)]
        public string SKU { get; set; }

        [Required(ErrorMessage = "Unit is required.")]
        [StringLength(20)]
        public string Unit { get; set; }

        [Required(ErrorMessage = "Selling Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Selling Price must be greater than zero.")]
        public decimal SellingPrice { get; set; }

        [Range(0, 100, ErrorMessage = "GST % must be between 0 and 100.")]
        public decimal? GSTPercentage { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<Sales_Invoice_Item> SalesInvoiceItems { get; set; }
            = new List<Sales_Invoice_Item>();
    }
}