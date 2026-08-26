using Sales_Billing_System.Models;
using Sales_Billing_System.Repositories;
using Sales_Billing_System.Repositories.Interfaces;
using Sales_Billing_System.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sales_Billing_System.Services
{
    public class SalesInvoiceService : ISalesInvoiceService
    {
        private readonly ISalesInvoiceRepository _salesInvoiceRepository;

        public SalesInvoiceService()
        {
            _salesInvoiceRepository = new SalesInvoiceRepository();
        }

        // Get all invoices
        public List<Sales_Invoice> GetAllInvoices()
        {
            return _salesInvoiceRepository.GetAllInvoices();
        }

        // Get invoice by ID
        public Sales_Invoice GetInvoiceById(int invoiceId)
        {
            return _salesInvoiceRepository.GetInvoiceById(invoiceId);
        }

        // Create new invoice
        public void CreateInvoice(Sales_Invoice invoice)
        {
            if (invoice == null)
            {
                throw new ArgumentNullException("invoice");
            }

            if (invoice.InvoiceItems == null ||
                !invoice.InvoiceItems.Any())
            {
                throw new Exception(
                    "Invoice must contain at least one item."
                );
            }

            // Set invoice date
            invoice.InvoiceDate = DateTime.Now;

            // Set invoice status
            invoice.Status = "Active";

            // Set created date
            invoice.CreatedAt = DateTime.Now;


            // Calculate Invoice Totals
            decimal totalTaxableAmount = 0;
            decimal totalGSTAmount = 0;


            foreach (Sales_Invoice_Item item in invoice.InvoiceItems)
            {
                // Calculate Taxable Amount
                item.TaxableAmount =
                    (item.Quantity * item.Rate) - item.Discount;

                // Prevent negative taxable amount
                if (item.TaxableAmount < 0)
                {
                    item.TaxableAmount = 0;
                }

                // Calculate GST Amount
                item.GSTAmount =
                    item.TaxableAmount *
                    item.GSTPercentage / 100;

                // Calculate Total Amount
                item.TotalAmount =
                    item.TaxableAmount +
                    item.GSTAmount;


                // Add to Invoice Totals
                totalTaxableAmount += item.TaxableAmount;

                totalGSTAmount += item.GSTAmount;
            }


            // Set Invoice Totals
            invoice.TotalTaxableAmount = totalTaxableAmount;

            invoice.TotalGSTAmount = totalGSTAmount;

            invoice.GrandTotal =
                invoice.TotalTaxableAmount +
                invoice.TotalGSTAmount;


            // Generate Invoice Number
            if (string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
            {
                invoice.InvoiceNumber =
                    GenerateInvoiceNumber();
            }


            // Save Invoice
            _salesInvoiceRepository.CreateInvoice(invoice);
        }


        // Generate Invoice Number
        private string GenerateInvoiceNumber()
        {
            return "INV-" +
                   DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}