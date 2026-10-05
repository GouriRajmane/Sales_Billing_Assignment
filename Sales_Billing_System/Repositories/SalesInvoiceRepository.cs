using MySql.Data.MySqlClient;
using Sales_Billing_System.Data;
using Sales_Billing_System.Models;
using Sales_Billing_System.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace Sales_Billing_System.Repositories
{
    public class SalesInvoiceRepository
        : ISalesInvoiceRepository
    {
        private readonly SalesBillingDbContext _context;


        public SalesInvoiceRepository()
        {
            _context =
                new SalesBillingDbContext();
        }


        // ============================================================
        // GET ALL INVOICES
        // ============================================================

        public List<Sales_Invoice> GetAllInvoices()
        {
            return _context.SalesInvoices
                .Include(i => i.Customer)
                .OrderByDescending(i => i.InvoiceDate)
                .ThenByDescending(i => i.InvoiceId)
                .ToList();
        }


        // ============================================================
        // SEARCH INVOICES
        // ============================================================

        public List<Sales_Invoice> SearchInvoices(
            string searchText,
            DateTime? fromDate,
            DateTime? toDate)
        {
            IQueryable<Sales_Invoice> query =
                _context.SalesInvoices
                    .Include(i => i.Customer);


            // --------------------------------------------------------
            // SEARCH TEXT
            // --------------------------------------------------------

            if (!string.IsNullOrWhiteSpace(
                searchText))
            {
                searchText =
                    searchText.Trim();


                query =
                    query.Where(i =>
                        i.InvoiceNumber.Contains(
                            searchText
                        )
                        ||
                        i.Customer.CustomerName.Contains(
                            searchText
                        )
                    );
            }


            // --------------------------------------------------------
            // FROM DATE
            // --------------------------------------------------------

            if (fromDate.HasValue)
            {
                DateTime startDate =
                    fromDate.Value.Date;


                query =
                    query.Where(i =>
                        i.InvoiceDate >= startDate
                    );
            }


            // --------------------------------------------------------
            // TO DATE
            // --------------------------------------------------------

            if (toDate.HasValue)
            {
                DateTime nextDay =
                    toDate.Value.Date.AddDays(1);


                query =
                    query.Where(i =>
                        i.InvoiceDate < nextDay
                    );
            }


            // --------------------------------------------------------
            // RESULT
            // --------------------------------------------------------

            return query
                .OrderByDescending(
                    i => i.InvoiceDate
                )
                .ThenByDescending(
                    i => i.InvoiceId
                )
                .ToList();
        }


        // ============================================================
        // GET INVOICE BY ID
        // ============================================================

        public Sales_Invoice GetInvoiceById(
            int invoiceId)
        {
            return _context.SalesInvoices
                .Include(i => i.Customer)
                .Include(
                    i => i.InvoiceItems
                        .Select(x => x.Product)
                )
                .FirstOrDefault(
                    i => i.InvoiceId == invoiceId
                );
        }


        // ============================================================
        // CREATE INVOICE
        // ============================================================

        public void CreateInvoice(
            Sales_Invoice invoice)
        {
            if (invoice == null)
            {
                throw new ArgumentNullException(
                    "invoice"
                );
            }


            _context.SalesInvoices.Add(
                invoice
            );


            _context.SaveChanges();
        }


        // ============================================================
        // CANCEL INVOICE
        // ============================================================

        public void CancelInvoice(
            int invoiceId)
        {
            Sales_Invoice invoice =
                _context.SalesInvoices
                    .FirstOrDefault(
                        i =>
                            i.InvoiceId ==
                            invoiceId
                    );


            if (invoice == null)
            {
                throw new Exception(
                    "Invoice not found."
                );
            }


            invoice.Status =
                "Cancelled";


            _context.SaveChanges();
        }


        // ============================================================
        // GENERATE INVOICE NUMBER
        // ============================================================

        public string GenerateInvoiceNumber()
        {
            int nextNumber;


            if (_context.SalesInvoices.Any())
            {
                nextNumber =
                    _context.SalesInvoices
                        .Max(
                            i => i.InvoiceId
                        ) + 1;
            }
            else
            {
                nextNumber = 1;
            }


            return "INV-" +
                   DateTime.Now.Year +
                   "-" +
                   nextNumber.ToString("D5");
        }

        public Sales_Invoice GetPrintInvoiceData(int invoiceId)
        {
            Sales_Invoice invoice = null;

            var connection = (MySqlConnection)_context.Database.Connection;

            bool shouldCloseConnection =
                connection.State != ConnectionState.Open;

            try
            {
                if (shouldCloseConnection)
                {
                    connection.Open();
                }

                using (MySqlCommand command = new MySqlCommand(
                    "sp_PrintInvoiceData",
                    connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue(
                        "@p_InvoiceId",
                        invoiceId
                    );

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Map invoice and customer only once.
                            if (invoice == null)
                            {
                                invoice = new Sales_Invoice
                                {
                                    InvoiceId = Convert.ToInt32(
                                        reader["InvoiceId"]
                                    ),

                                    InvoiceNumber = Convert.ToString(
                                        reader["InvoiceNumber"]
                                    ),

                                    InvoiceDate = Convert.ToDateTime(
                                        reader["InvoiceDate"]
                                    ),

                                    Status = Convert.ToString(
                                        reader["Status"]
                                    ),

                                    CreatedAt = Convert.ToDateTime(
                                        reader["CreatedAt"]
                                    ),

                                    TotalTaxableAmount = Convert.ToDecimal(
                                        reader["TotalTaxableAmount"]
                                    ),

                                    TotalGSTAmount = Convert.ToDecimal(
                                        reader["TotalGSTAmount"]
                                    ),

                                    GrandTotal = Convert.ToDecimal(
                                        reader["GrandTotal"]
                                    ),

                                    CustomerId = Convert.ToInt32(
                                        reader["CustomerId"]
                                    ),

                                    Customer = new Customer_Master
                                    {
                                        CustomerId = Convert.ToInt32(
                                            reader["CustomerId"]
                                        ),

                                        CustomerName = Convert.ToString(
                                            reader["CustomerName"]
                                        ),

                                        MobileNumber = Convert.ToString(
                                            reader["MobileNumber"]
                                        ),

                                        Address = reader["Address"] == DBNull.Value
                                            ? null
                                            : Convert.ToString(reader["Address"]),

                                        GSTIN = reader["GSTIN"] == DBNull.Value
                                            ? null
                                            : Convert.ToString(reader["GSTIN"])
                                    },

                                    InvoiceItems =
                                        new List<Sales_Invoice_Item>()
                                };
                            }

                            // Map each invoice item and its product.
                            Sales_Invoice_Item item =
                                new Sales_Invoice_Item
                                {
                                    InvoiceItemId = Convert.ToInt32(
                                        reader["InvoiceItemId"]
                                    ),

                                    InvoiceId = invoice.InvoiceId,

                                    ProductId = Convert.ToInt32(
                                        reader["ProductId"]
                                    ),

                                    Quantity = Convert.ToDecimal(
                                        reader["Quantity"]
                                    ),

                                    Rate = Convert.ToDecimal(
                                        reader["Rate"]
                                    ),

                                    Discount = Convert.ToDecimal(
                                        reader["Discount"]
                                    ),

                                    GSTPercentage = Convert.ToDecimal(
                                        reader["GSTPercentage"]
                                    ),

                                    TaxableAmount = Convert.ToDecimal(
                                        reader["TaxableAmount"]
                                    ),

                                    GSTAmount = Convert.ToDecimal(
                                        reader["GSTAmount"]
                                    ),

                                    TotalAmount = Convert.ToDecimal(
                                        reader["TotalAmount"]
                                    ),

                                    Product = new Product_Master
                                    {
                                        ProductId = Convert.ToInt32(
                                            reader["ProductId"]
                                        ),

                                        ProductName = Convert.ToString(
                                            reader["ProductName"]
                                        ),

                                        SKU = reader["SKU"] == DBNull.Value
                                            ? null
                                            : Convert.ToString(reader["SKU"]),

                                        Unit = reader["Unit"] == DBNull.Value
                                            ? null
                                            : Convert.ToString(reader["Unit"])
                                    }
                                };

                            invoice.InvoiceItems.Add(item);
                        }
                    }
                }
            }
            finally
            {
                if (shouldCloseConnection &&
                    connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return invoice;
        }

        public PagedResult<Sales_Invoice> GetInvoicesPaged(
    int pageNumber,
    int pageSize,
    string searchText,
    DateTime? fromDate,
    DateTime? toDate)
        {
            var result =
                new PagedResult<Sales_Invoice>();

            result.CurrentPage = pageNumber;
            result.PageSize = pageSize;

            var connection =
                (MySqlConnection)_context.Database.Connection;

            bool shouldCloseConnection =
                connection.State != ConnectionState.Open;

            try
            {
                if (shouldCloseConnection)
                {
                    connection.Open();
                }

                using (MySqlCommand command =
                    new MySqlCommand(
                        "sp_GetInvoicesPaged",
                        connection))
                {
                    command.CommandType =
                        CommandType.StoredProcedure;

                    // -----------------------------------------
                    // PAGE NUMBER
                    // -----------------------------------------

                    command.Parameters.AddWithValue(
                        "@p_PageNumber",
                        pageNumber);


                    // -----------------------------------------
                    // PAGE SIZE
                    // -----------------------------------------

                    command.Parameters.AddWithValue(
                        "@p_PageSize",
                        pageSize);


                    // -----------------------------------------
                    // SEARCH TEXT
                    // -----------------------------------------

                    command.Parameters.AddWithValue(
                        "@p_SearchText",
                        string.IsNullOrWhiteSpace(searchText)
                            ? (object)DBNull.Value
                            : searchText.Trim());


                    // -----------------------------------------
                    // FROM DATE
                    // -----------------------------------------

                    command.Parameters.AddWithValue(
                        "@p_FromDate",
                        fromDate.HasValue
                            ? (object)fromDate.Value.Date
                            : DBNull.Value);


                    // -----------------------------------------
                    // TO DATE
                    // -----------------------------------------

                    command.Parameters.AddWithValue(
                        "@p_ToDate",
                        toDate.HasValue
                            ? (object)toDate.Value.Date
                            : DBNull.Value);


                    using (MySqlDataReader reader =
                        command.ExecuteReader())
                    {
                        // =====================================
                        // RESULT SET 1
                        // INVOICE DATA
                        // =====================================

                        while (reader.Read())
                        {
                            Sales_Invoice invoice =
                                new Sales_Invoice
                                {
                                    InvoiceId =
                                        Convert.ToInt32(
                                            reader["InvoiceId"]),

                                    InvoiceNumber =
                                        Convert.ToString(
                                            reader["InvoiceNumber"]),

                                    InvoiceDate =
                                        Convert.ToDateTime(
                                            reader["InvoiceDate"]),

                                    CustomerId =
                                        Convert.ToInt32(
                                            reader["CustomerId"]),

                                    TotalTaxableAmount =
                                        Convert.ToDecimal(
                                            reader["TotalTaxableAmount"]),

                                    TotalGSTAmount =
                                        Convert.ToDecimal(
                                            reader["TotalGSTAmount"]),

                                    GrandTotal =
                                        Convert.ToDecimal(
                                            reader["GrandTotal"]),

                                    Status =
                                        Convert.ToString(
                                            reader["Status"]),

                                    CreatedAt =
                                        Convert.ToDateTime(
                                            reader["CreatedAt"]),

                                    Customer =
                                        new Customer_Master
                                        {
                                            CustomerId =
                                                Convert.ToInt32(
                                                    reader["CustomerId"]),

                                            CustomerName =
                                                Convert.ToString(
                                                    reader["CustomerName"])
                                        }
                                };

                            result.Items.Add(invoice);
                        }


                        // =====================================
                        // RESULT SET 2
                        // TOTAL RECORDS
                        // =====================================

                        if (reader.NextResult() &&
                            reader.Read())
                        {
                            result.TotalRecords =
                                Convert.ToInt32(
                                    reader["TotalRecords"]);
                        }
                    }
                }
            }
            finally
            {
                if (shouldCloseConnection &&
                    connection.State ==
                        ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }
    }
}