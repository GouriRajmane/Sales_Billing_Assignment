using Sales_Billing_System.Data;
using Sales_Billing_System.Models;
using Sales_Billing_System.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Sales_Billing_System.Repositories
{
    public class SalesInvoiceRepository : ISalesInvoiceRepository
    {
        private readonly SalesBillingDbContext _context;

        public SalesInvoiceRepository()
        {
            _context = new SalesBillingDbContext();
        }

        public List<Sales_Invoice> GetAllInvoices()
        {
            return _context.SalesInvoices
                .Include(i => i.Customer)
                .OrderByDescending(i => i.InvoiceDate)
                //.ThenByDescending(i => i.InvoiceId)
                .ToList();
        }

        public List<Sales_Invoice> SearchInvoices(
            string searchText,
            DateTime? fromDate,
            DateTime? toDate)
        {
            IQueryable<Sales_Invoice> query =
                _context.SalesInvoices
                    .Include(i => i.Customer);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(i =>
                    i.InvoiceNumber.Contains(searchText) ||
                    i.Customer.CustomerName.Contains(searchText));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(i =>
                    i.InvoiceDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                DateTime nextDay =
                    toDate.Value.Date.AddDays(1);

                query = query.Where(i =>
                    i.InvoiceDate < nextDay);
            }

            return query
                .OrderByDescending(i => i.InvoiceDate)
                .ThenByDescending(i => i.InvoiceId)
                .ToList();
        }

        public Sales_Invoice GetInvoiceById(int invoiceId)
        {
            return _context.SalesInvoices
                .Include(i => i.Customer)
                .Include(i => i.InvoiceItems.Select(x => x.Product))
                .FirstOrDefault(i =>
                    i.InvoiceId == invoiceId);
        }

        public void CreateInvoice(Sales_Invoice invoice)
        {
            _context.SalesInvoices.Add(invoice);
            _context.SaveChanges();
        }

        public void CancelInvoice(int invoiceId)
        {
            Sales_Invoice invoice =
                _context.SalesInvoices
                    .FirstOrDefault(i =>
                        i.InvoiceId == invoiceId);

            if (invoice != null)
            {
                invoice.Status = "Cancelled";

                _context.SaveChanges();
            }
        }

        public string GenerateInvoiceNumber()
        {
            int nextNumber =
                _context.SalesInvoices.Any()
                    ? _context.SalesInvoices
                        .Max(i => i.InvoiceId) + 1
                    : 1;

            return "INV-" +
                   DateTime.Now.Year +
                   "-" +
                   nextNumber.ToString("D5");
        }
    }
}