using Sales_Billing_System.Data;
using Sales_Billing_System.Models;
using Sales_Billing_System.Repositories.Interfaces;
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

        // Get all invoices
        public List<Sales_Invoice> GetAllInvoices()
        {
            return _context.SalesInvoices
                           .Include(i => i.Customer)
                           .Include(i => i.InvoiceItems)
                           .OrderByDescending(i => i.InvoiceId)
                           .ToList();
        }

        // Get invoice by ID
        public Sales_Invoice GetInvoiceById(int invoiceId)
        {
            return _context.SalesInvoices
                           .Include(i => i.Customer)
                           .Include(i => i.InvoiceItems)
                           .Include(i => i.InvoiceItems.Select(item => item.Product))
                           .FirstOrDefault(i => i.InvoiceId == invoiceId);
        }

        // Create invoice with multiple items
        public void CreateInvoice(Sales_Invoice invoice)
        {
            _context.SalesInvoices.Add(invoice);

            _context.SaveChanges();
        }
    }
}