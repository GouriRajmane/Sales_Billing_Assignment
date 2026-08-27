using Sales_Billing_System.Models;
using System;
using System.Collections.Generic;

namespace Sales_Billing_System.Services.Interfaces
{
    public interface ISalesInvoiceService
    {
        List<Sales_Invoice> GetAllInvoices();

        List<Sales_Invoice> SearchInvoices(
            string searchText,
            DateTime? fromDate,
            DateTime? toDate
        );

        Sales_Invoice GetInvoiceById(int invoiceId);

        void CreateInvoice(SalesInvoiceViewModel model);

        void CancelInvoice(int invoiceId);

        string GenerateInvoiceNumber();
    }
}