using Sales_Billing_System.Models;
using Sales_Billing_System.Services;
using Sales_Billing_System.Services.Interfaces;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Sales_Billing_System.Controllers
{
    public class SalesInvoiceController : Controller
    {
        private readonly ISalesInvoiceService _salesInvoiceService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;

        public SalesInvoiceController()
        {
            _salesInvoiceService = new SalesInvoiceService();
            _customerService = new CustomerService();
            _productService = new ProductService();
        }

        // GET: SalesInvoice
        public ActionResult Index()
        {
            var invoices = _salesInvoiceService.GetAllInvoices();

            return View(invoices);
        }

        // GET: SalesInvoice/Details/5
        public ActionResult Details(int id)
        {
            Sales_Invoice invoice =
                _salesInvoiceService.GetInvoiceById(id);

            if (invoice == null)
            {
                return HttpNotFound();
            }

            return View(invoice);
        }

        // GET: SalesInvoice/Create
        public ActionResult Create()
        {
            LoadDropdowns();

            Sales_Invoice invoice = new Sales_Invoice();

            invoice.InvoiceNumber = GenerateInvoiceNumber();

            invoice.InvoiceDate = DateTime.Now;

            return View(invoice);
        }

        // POST: SalesInvoice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Sales_Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _salesInvoiceService.CreateInvoice(invoice);

                    TempData["SuccessMessage"] =
                        "Sales invoice created successfully.";

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(
                        "",
                        "Unable to create invoice. " + ex.Message
                    );
                }
            }

            LoadDropdowns();

            return View(invoice);
        }

        // Load Customer and Product Dropdowns
        private void LoadDropdowns()
        {
            var customers = _customerService
                .GetAllCustomers()
                .Select(c => new SelectListItem
                {
                    Value = c.CustomerId.ToString(),
                    Text = c.CustomerName
                })
                .ToList();

            var products = _productService
                .GetAllProducts()
                .Where(p => p.IsActive)
                .Select(p => new SelectListItem
                {
                    Value = p.ProductId.ToString(),
                    Text = p.ProductName + " (" + p.SKU + ")"
                })
                .ToList();

            ViewBag.Customers = customers;
            ViewBag.Products = products;
        }

        // Generate Invoice Number for Display
        private string GenerateInvoiceNumber()
        {
            return "INV-" +
                   DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}