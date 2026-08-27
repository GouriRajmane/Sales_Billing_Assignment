using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sales_Billing_System.Models
{
    public class Sales_Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public decimal TotalTaxableAmount { get; set; }

        public decimal TotalGSTAmount { get; set; }

        public decimal GrandTotal { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation Property
        public virtual Customer_Master Customer { get; set; }

        // One Invoice -> Many Invoice Items
        public virtual ICollection<Sales_Invoice_Item> InvoiceItems { get; set; }

        public Sales_Invoice()
        {
            InvoiceDate = DateTime.Now;
            Status = "Active";
            CreatedAt = DateTime.Now;
            InvoiceItems = new List<Sales_Invoice_Item>();
        }
    }
}