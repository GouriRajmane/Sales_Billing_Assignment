using System;
using System.Collections.Generic;

namespace Sales_Billing_System.Models.ViewModels
{
    public class SalesInvoiceViewModel
    {
        // Invoice Header
        public int InvoiceId { get; set; }

        public string InvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        public int CustomerId { get; set; }

        // Invoice Totals
        public decimal TotalTaxableAmount { get; set; }

        public decimal TotalGSTAmount { get; set; }

        public decimal GrandTotal { get; set; }

        // Invoice Items
        public List<SalesInvoiceItemViewModel> InvoiceItems { get; set; }

        public SalesInvoiceViewModel()
        {
            InvoiceItems = new List<SalesInvoiceItemViewModel>();
            InvoiceDate = DateTime.Now;
        }
    }
}