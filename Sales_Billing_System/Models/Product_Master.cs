using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sales_Billing_System.Models
{
    public class Product_Master
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        [StringLength(50)]
        public string SKU { get; set; }

        [Required]
        [StringLength(20)]
        public string Unit { get; set; }

        [Required]
        [Range(
            0,
            double.MaxValue,
            ErrorMessage = "Selling Price must be a positive value."
        )]
        public decimal SellingPrice { get; set; }

        [Required]
        [Range(
            0,
            100,
            ErrorMessage = "GST % must be between 0 and 100."
        )]
        public decimal GSTPercentage { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        // One Product -> Many Invoice Items
        public virtual ICollection<Sales_Invoice_Item> SalesInvoiceItems { get; set; }

        public Product_Master()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;

            SalesInvoiceItems = new List<Sales_Invoice_Item>();
        }
    }
}