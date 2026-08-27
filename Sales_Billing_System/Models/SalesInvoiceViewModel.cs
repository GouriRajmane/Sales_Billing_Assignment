using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sales_Billing_System.Models
{
    public class SalesInvoiceViewModel
    {
        public SalesInvoiceViewModel()
        {
            InvoiceDate = DateTime.Today;

            Items =
                new List<Sales_Invoice_Item>();
        }

        public int InvoiceId { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }

        [Required(
            ErrorMessage =
            "Please select a customer."
        )]
        public int CustomerId { get; set; }

        public List<Sales_Invoice_Item> Items { get; set; }

        public decimal TotalTaxableAmount { get; set; }

        public decimal TotalGSTAmount { get; set; }

        public decimal GrandTotal { get; set; }
    }
}