using System.ComponentModel.DataAnnotations;

namespace Sales_Billing_System.Models
{
    public class Sales_Invoice_Item
    {
        [Key]
        public int InvoiceItemId { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue,
            ErrorMessage = "Quantity must be greater than zero.")]
        public decimal Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue,
            ErrorMessage = "Rate cannot be negative.")]
        public decimal Rate { get; set; }

        [Range(0, double.MaxValue,
            ErrorMessage = "Discount cannot be negative.")]
        public decimal Discount { get; set; }

        [Range(0, 100,
            ErrorMessage = "GST % must be between 0 and 100.")]
        public decimal GSTPercentage { get; set; }

        public decimal TaxableAmount { get; set; }

        public decimal GSTAmount { get; set; }

        public decimal TotalAmount { get; set; }

        // Navigation Properties
        public virtual Sales_Invoice SalesInvoice { get; set; }

        public virtual Product_Master Product { get; set; }
    }
}