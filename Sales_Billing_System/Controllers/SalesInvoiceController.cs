using Sales_Billing_System.Models;
using Sales_Billing_System.Services;
using Sales_Billing_System.Services.Interfaces;
using System;
using System.Web.Mvc;

namespace Sales_Billing_System.Controllers
{
    public class SalesInvoiceController : Controller
    {
        private readonly ISalesInvoiceService
            _invoiceService;

        private readonly ICustomerService
            _customerService;

        private readonly IProductService
            _productService;

        public SalesInvoiceController()
        {
            _invoiceService =
                new SalesInvoiceService();

            _customerService =
                new CustomerService();

            _productService =
                new ProductService();
        }

        // GET: SalesInvoice
        public ActionResult Index(
            string searchText,
            DateTime? fromDate,
            DateTime? toDate)
        {
            ViewBag.SearchText =
                searchText;

            ViewBag.FromDate =
                fromDate;

            ViewBag.ToDate =
                toDate;

            var invoices =
                _invoiceService.SearchInvoices(
                    searchText,
                    fromDate,
                    toDate
                );

            return View(invoices);
        }

        // GET: SalesInvoice/Create
        [HttpGet]
        public ActionResult Create()
        {
            LoadDropdownData();

            SalesInvoiceViewModel model =
                new SalesInvoiceViewModel();

            model.InvoiceNumber =
                _invoiceService
                    .GenerateInvoiceNumber();

            model.Items.Add(
                new Sales_Invoice_Item()
            );

            return View(model);
        }

        // POST: SalesInvoice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            SalesInvoiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdownData();

                return View(model);
            }

            try
            {
                _invoiceService
                    .CreateInvoice(model);

                TempData["SuccessMessage"] =
                    "Invoice created successfully.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LoadDropdownData();

                ModelState.AddModelError(
                    "",
                    ex.Message
                );

                return View(model);
            }
        }

        // GET: SalesInvoice/Details/5
        public ActionResult Details(int id)
        {
            Sales_Invoice invoice =
                _invoiceService
                    .GetInvoiceById(id);

            if (invoice == null)
            {
                return HttpNotFound();
            }

            return View(invoice);
        }

        // GET: SalesInvoice/Print/5
        public ActionResult Print(int id)
        {
            Sales_Invoice invoice =
                _invoiceService
                    .GetInvoiceById(id);

            if (invoice == null)
            {
                return HttpNotFound();
            }

            return View(invoice);
        }

        // POST: SalesInvoice/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(int id)
        {
            try
            {
                _invoiceService.CancelInvoice(id);

                TempData["SuccessMessage"] =
                    "Invoice cancelled successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    ex.Message;
            }

            return RedirectToAction("Index");
        }

        private void LoadDropdownData()
        {
            ViewBag.Customers =
                new SelectList(
                    _customerService
                        .GetAllCustomers(),
                    "CustomerId",
                    "CustomerName"
                );

            ViewBag.Products =
                new SelectList(
                    _productService
                        .GetActiveProducts(),
                    "ProductId",
                    "ProductName"
                );
        }
    }
}