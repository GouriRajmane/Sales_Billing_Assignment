
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Sales_Billing_System.Models
{
    public class SalesInvoiceViewModel
    {
        public SalesInvoiceViewModel()
        {
            InvoiceDate = DateTime.Today;

            Items = new List<Sales_Invoice_Item>();

            Customers = new List<SelectListItem>();

            Products = new List<Product_Master>();
        }

        public int InvoiceId { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }

        [Required(
            ErrorMessage = "Please select a customer."
        )]
        public int CustomerId { get; set; }

        public List<Sales_Invoice_Item> Items { get; set; }

        public decimal TotalTaxableAmount { get; set; }

        public decimal TotalGSTAmount { get; set; }

        public decimal GrandTotal { get; set; }

        // Dropdown data
        public List<SelectListItem> Customers { get; set; }

        public List<Product_Master> Products { get; set; }
    }
}