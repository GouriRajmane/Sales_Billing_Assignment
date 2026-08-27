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
        private readonly ISalesInvoiceRepository _invoiceRepository;

        public SalesInvoiceService()
        {
            _invoiceRepository =
                new SalesInvoiceRepository();
        }

        public List<Sales_Invoice> GetAllInvoices()
        {
            return _invoiceRepository.GetAllInvoices();
        }

        public List<Sales_Invoice> SearchInvoices(
            string searchText,
            DateTime? fromDate,
            DateTime? toDate)
        {
            return _invoiceRepository.SearchInvoices(
                searchText,
                fromDate,
                toDate
            );
        }

        public Sales_Invoice GetInvoiceById(int invoiceId)
        {
            return _invoiceRepository.GetInvoiceById(invoiceId);
        }

        public string GenerateInvoiceNumber()
        {
            return _invoiceRepository.GenerateInvoiceNumber();
        }

        public void CreateInvoice(SalesInvoiceViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            if (model.CustomerId <= 0)
            {
                throw new Exception(
                    "Please select a customer."
                );
            }

            if (model.Items == null ||
                !model.Items.Any())
            {
                throw new Exception(
                    "Invoice must contain at least one item."
                );
            }

            Sales_Invoice invoice =
                new Sales_Invoice();

            invoice.InvoiceNumber =
                model.InvoiceNumber;

            invoice.InvoiceDate =
                model.InvoiceDate;

            invoice.CustomerId =
                model.CustomerId;

            invoice.Status =
                "Active";

            invoice.CreatedAt =
                DateTime.Now;

            foreach (Sales_Invoice_Item item
                in model.Items)
            {
                if (item.ProductId <= 0)
                {
                    throw new Exception(
                        "Please select a product."
                    );
                }

                if (item.Quantity <= 0)
                {
                    throw new Exception(
                        "Quantity must be greater than zero."
                    );
                }

                if (item.Rate < 0)
                {
                    throw new Exception(
                        "Rate cannot be negative."
                    );
                }

                if (item.Discount < 0)
                {
                    throw new Exception(
                        "Discount cannot be negative."
                    );
                }

                if (item.GSTPercentage < 0 ||
                    item.GSTPercentage > 100)
                {
                    throw new Exception(
                        "GST percentage must be between 0 and 100."
                    );
                }

                decimal lineAmount =
                    item.Quantity * item.Rate;

                decimal taxableAmount =
                    lineAmount - item.Discount;

                if (taxableAmount < 0)
                {
                    throw new Exception(
                        "Discount cannot be greater than the line amount."
                    );
                }

                decimal gstAmount =
                    taxableAmount *
                    item.GSTPercentage / 100;

                decimal totalAmount =
                    taxableAmount +
                    gstAmount;

                Sales_Invoice_Item invoiceItem =
                    new Sales_Invoice_Item();

                invoiceItem.ProductId =
                    item.ProductId;

                invoiceItem.Quantity =
                    item.Quantity;

                invoiceItem.Rate =
                    item.Rate;

                invoiceItem.Discount =
                    item.Discount;

                invoiceItem.GSTPercentage =
                    item.GSTPercentage;

                invoiceItem.TaxableAmount =
                    Math.Round(taxableAmount, 2);

                invoiceItem.GSTAmount =
                    Math.Round(gstAmount, 2);

                invoiceItem.TotalAmount =
                    Math.Round(totalAmount, 2);

                invoice.InvoiceItems.Add(
                    invoiceItem
                );
            }

            invoice.TotalTaxableAmount =
                Math.Round(
                    invoice.InvoiceItems.Sum(
                        i => i.TaxableAmount
                    ),
                    2
                );

            invoice.TotalGSTAmount =
                Math.Round(
                    invoice.InvoiceItems.Sum(
                        i => i.GSTAmount
                    ),
                    2
                );

            invoice.GrandTotal =
                Math.Round(
                    invoice.TotalTaxableAmount +
                    invoice.TotalGSTAmount,
                    2
                );

            _invoiceRepository.CreateInvoice(
                invoice
            );
        }

        public void CancelInvoice(int invoiceId)
        {
            Sales_Invoice invoice =
                _invoiceRepository
                    .GetInvoiceById(invoiceId);

            if (invoice == null)
            {
                throw new Exception(
                    "Invoice not found."
                );
            }

            if (invoice.Status == "Cancelled")
            {
                throw new Exception(
                    "Invoice is already cancelled."
                );
            }

            _invoiceRepository.CancelInvoice(
                invoiceId
            );
        }
    }
}