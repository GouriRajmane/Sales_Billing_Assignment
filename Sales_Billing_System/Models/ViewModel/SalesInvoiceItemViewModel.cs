using System.ComponentModel.DataAnnotations;

namespace Sales_Billing_System.Models.ViewModels
{
    public class SalesInvoiceItemViewModel
    {
        public int InvoiceItemId { get; set; }

        public int ProductId { get; set; }

        [Range(0.01, double.MaxValue,
            ErrorMessage = "Quantity must be greater than zero.")]
        public decimal Quantity { get; set; }

        public decimal Rate { get; set; }

        public decimal Discount { get; set; }

        public decimal GSTPercentage { get; set; }

        public decimal TaxableAmount { get; set; }

        public decimal GSTAmount { get; set; }

        public decimal TotalAmount { get; set; }
    }
}