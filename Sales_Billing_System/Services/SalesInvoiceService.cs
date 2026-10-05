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


        // ============================================================
        // GET ALL
        // ============================================================

        public List<Sales_Invoice> GetAllInvoices()
        {
            return _invoiceRepository.GetAllInvoices();
        }


        // ============================================================
        // SEARCH
        // ============================================================

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


        // ============================================================
        // GET BY ID
        // ============================================================

        public Sales_Invoice GetInvoiceById(
            int invoiceId)
        {
            return _invoiceRepository
                .GetInvoiceById(invoiceId);
        }


        // ============================================================
        // GENERATE INVOICE NUMBER
        // ============================================================

        public string GenerateInvoiceNumber()
        {
            return _invoiceRepository
                .GenerateInvoiceNumber();
        }


        // ============================================================
        // CREATE INVOICE
        // ============================================================

        public void CreateInvoice(
            SalesInvoiceViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(
                    "model"
                );
            }


            // --------------------------------------------------------
            // CUSTOMER VALIDATION
            // --------------------------------------------------------

            if (model.CustomerId <= 0)
            {
                throw new Exception(
                    "Please select a customer."
                );
            }


            // --------------------------------------------------------
            // ITEM VALIDATION
            // --------------------------------------------------------

            if (model.Items == null ||
                !model.Items.Any())
            {
                throw new Exception(
                    "Invoice must contain at least one item."
                );
            }


            // --------------------------------------------------------
            // CREATE INVOICE
            // --------------------------------------------------------

            Sales_Invoice invoice =
                new Sales_Invoice();


            /*
             * Make absolutely sure the collection exists.
             */
            if (invoice.InvoiceItems == null)
            {
                invoice.InvoiceItems =
                    new List<Sales_Invoice_Item>();
            }


            invoice.InvoiceNumber =
                model.InvoiceNumber;


            /*
             * If invoice number is empty,
             * generate a new one.
             */
            if (string.IsNullOrWhiteSpace(
                invoice.InvoiceNumber))
            {
                invoice.InvoiceNumber =
                    GenerateInvoiceNumber();
            }


            invoice.InvoiceDate =
                model.InvoiceDate;


            invoice.CustomerId =
                model.CustomerId;


            invoice.Status =
                "Active";


            invoice.CreatedAt =
                DateTime.Now;


            // --------------------------------------------------------
            // PROCESS EACH ITEM
            // --------------------------------------------------------

            foreach (
                Sales_Invoice_Item item
                in model.Items)
            {
                if (item == null)
                {
                    continue;
                }


                // ----------------------------------------------------
                // PRODUCT
                // ----------------------------------------------------

                if (item.ProductId <= 0)
                {
                    throw new Exception(
                        "Please select a product."
                    );
                }


                // ----------------------------------------------------
                // QUANTITY
                // ----------------------------------------------------

                if (item.Quantity <= 0)
                {
                    throw new Exception(
                        "Quantity must be greater than zero."
                    );
                }


                // ----------------------------------------------------
                // RATE
                // ----------------------------------------------------

                if (item.Rate < 0)
                {
                    throw new Exception(
                        "Rate cannot be negative."
                    );
                }


                // ----------------------------------------------------
                // DISCOUNT
                // ----------------------------------------------------

                if (item.Discount < 0)
                {
                    throw new Exception(
                        "Discount cannot be negative."
                    );
                }


                // ----------------------------------------------------
                // GST
                // ----------------------------------------------------

                if (
                    item.GSTPercentage < 0 ||
                    item.GSTPercentage > 100
                )
                {
                    throw new Exception(
                        "GST percentage must be between 0 and 100."
                    );
                }


                // ====================================================
                // CALCULATION
                // ====================================================

                /*
                 * STEP 1
                 *
                 * Quantity × Rate
                 */

                decimal lineAmount =
                    item.Quantity *
                    item.Rate;


                /*
                 * STEP 2
                 *
                 * Taxable Amount
                 *
                 * (Quantity × Rate) - Discount
                 */

                decimal taxableAmount =
                    lineAmount -
                    item.Discount;


                /*
                 * Discount cannot exceed
                 * the line amount.
                 */

                if (taxableAmount < 0)
                {
                    throw new Exception(
                        "Discount cannot be greater than the line amount."
                    );
                }


                /*
                 * STEP 3
                 *
                 * Round taxable amount.
                 */

                taxableAmount =
                    Math.Round(
                        taxableAmount,
                        2,
                        MidpointRounding.AwayFromZero
                    );


                /*
                 * STEP 4
                 *
                 * GST Amount
                 *
                 * Taxable × GST / 100
                 */

                decimal gstAmount =
                    taxableAmount *
                    item.GSTPercentage /
                    100;


                gstAmount =
                    Math.Round(
                        gstAmount,
                        2,
                        MidpointRounding.AwayFromZero
                    );


                /*
                 * STEP 5
                 *
                 * Total Amount
                 *
                 * Taxable + GST
                 */

                decimal totalAmount =
                    taxableAmount +
                    gstAmount;


                totalAmount =
                    Math.Round(
                        totalAmount,
                        2,
                        MidpointRounding.AwayFromZero
                    );


                // ====================================================
                // CREATE INVOICE ITEM
                // ====================================================

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
                    taxableAmount;


                invoiceItem.GSTAmount =
                    gstAmount;


                invoiceItem.TotalAmount =
                    totalAmount;


                invoice.InvoiceItems.Add(
                    invoiceItem
                );
            }


            // ========================================================
            // INVOICE TOTALS
            // ========================================================

            /*
             * Total Taxable Amount
             */

            invoice.TotalTaxableAmount =
                Math.Round(
                    invoice.InvoiceItems.Sum(
                        x => x.TaxableAmount
                    ),
                    2,
                    MidpointRounding.AwayFromZero
                );


            /*
             * Total GST Amount
             */

            invoice.TotalGSTAmount =
                Math.Round(
                    invoice.InvoiceItems.Sum(
                        x => x.GSTAmount
                    ),
                    2,
                    MidpointRounding.AwayFromZero
                );


            /*
             * Grand Total
             */

            invoice.GrandTotal =
                Math.Round(
                    invoice.TotalTaxableAmount +
                    invoice.TotalGSTAmount,
                    2,
                    MidpointRounding.AwayFromZero
                );


            // ========================================================
            // SAVE
            // ========================================================

            _invoiceRepository.CreateInvoice(
                invoice
            );
        }


        // ============================================================
        // CANCEL INVOICE
        // ============================================================

        public void CancelInvoice(
            int invoiceId)
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


            if (
                invoice.Status != null &&
                invoice.Status.Equals(
                    "Cancelled",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                throw new Exception(
                    "Invoice is already cancelled."
                );
            }


            _invoiceRepository.CancelInvoice(
                invoiceId
            );
        }


        // ============================================================
        // PRINT INVOICE using SP
        // ============================================================
        public Sales_Invoice GetPrintInvoiceData(int invoiceId)
        {
            if (invoiceId <= 0)
            {
                throw new ArgumentException(
                    "Invalid invoice ID.",
                    "invoiceId"
                );
            }

            return _invoiceRepository.GetPrintInvoiceData(invoiceId);
        }

        public PagedResult<Sales_Invoice> GetInvoicesPaged(
            int pageNumber,
            int pageSize,
            string searchText,
            DateTime? fromDate,
            DateTime? toDate)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 10;
            }

            if (pageSize > 100)
            {
                pageSize = 100;
            }

            return _invoiceRepository.GetInvoicesPaged(
                pageNumber,
                pageSize,
                searchText,
                fromDate,
                toDate);
        }
    }
}
