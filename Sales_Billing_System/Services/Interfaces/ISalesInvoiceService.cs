using Sales_Billing_System.Models;
using System.Collections.Generic;

namespace Sales_Billing_System.Services.Interfaces
{
    public interface ISalesInvoiceService
    {
        List<Sales_Invoice> GetAllInvoices();

        Sales_Invoice GetInvoiceById(int invoiceId);

        void CreateInvoice(Sales_Invoice invoice);
    }
}