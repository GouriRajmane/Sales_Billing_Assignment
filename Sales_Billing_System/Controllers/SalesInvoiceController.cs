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
        private readonly ISalesInvoiceService _invoiceService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;

        public SalesInvoiceController()
        {
            _invoiceService = new SalesInvoiceService();
            _customerService = new CustomerService();
            _productService = new ProductService();
        }

        // INDEX
        public ActionResult Index(
            string searchText,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 10)
        {
            ViewBag.SearchText = searchText;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.PageSize = pageSize;

            var invoices = _invoiceService.GetInvoicesPaged(
                page,
                pageSize,
                searchText,
                fromDate,
                toDate);

            return View(invoices);
        }

        // CREATE - GET (loads into modal)
        [HttpGet]
        public ActionResult Create()
        {
            SalesInvoiceViewModel model = new SalesInvoiceViewModel();

            model.InvoiceNumber = _invoiceService.GenerateInvoiceNumber();
            model.Items.Add(new Sales_Invoice_Item());

            LoadDropdownData(model);

            return PartialView(model);
        }

        // CREATE - POST (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SalesInvoiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdownData(model);
                return PartialView(model);
            }

            try
            {
                _invoiceService.CreateInvoice(model);

                return Json(new
                {
                    success = true,
                    message = "Invoice created successfully."
                });
            }
            catch (Exception ex)
            {
                LoadDropdownData(model);
                ModelState.AddModelError("", ex.Message);

                return PartialView(model);
            }
        }

        // DETAILS (loads into modal)
        [HttpGet]
        public ActionResult Details(int id)
        {
            Sales_Invoice invoice = _invoiceService.GetInvoiceById(id);

            if (invoice == null)
            {
                return HttpNotFound();
            }

            return PartialView(invoice);
        }

        // PRINT (full page, opens in new tab)
        public ActionResult Print(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(400, "Invalid invoice ID.");
            }

            Sales_Invoice invoice = _invoiceService.GetPrintInvoiceData(id);

            if (invoice == null)
            {
                return HttpNotFound("Invoice not found.");
            }

            return View(invoice);
        }

        // CANCEL - confirmation modal (GET)
        [HttpGet]
        public ActionResult ConfirmCancel(int id)
        {
            Sales_Invoice invoice = _invoiceService.GetInvoiceById(id);

            if (invoice == null)
            {
                return HttpNotFound();
            }

            return PartialView(invoice);
        }

        // CANCEL - POST (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(int id)
        {
            try
            {
                _invoiceService.CancelInvoice(id);

                return Json(new
                {
                    success = true,
                    message = "Invoice cancelled successfully."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // GET PRODUCT DETAILS (kept for optional AJAX use; Create view does not need it)
        [HttpGet]
        public JsonResult GetProductDetails(int id)
        {
            var product = _productService.GetProductById(id);

            if (product == null || !product.IsActive)
            {
                return Json(
                    new { success = false, message = "Product not found or inactive." },
                    JsonRequestBehavior.AllowGet);
            }

            return Json(
                new
                {
                    success = true,
                    productId = product.ProductId,
                    productName = product.ProductName,
                    rate = product.SellingPrice,
                    gstPercentage = product.GSTPercentage
                },
                JsonRequestBehavior.AllowGet);
        }

        // LOAD DROPDOWN DATA
        private void LoadDropdownData(SalesInvoiceViewModel model)
        {
            model.Customers = _customerService
                .GetAllCustomers()
                .Select(c => new SelectListItem
                {
                    Value = c.CustomerId.ToString(),
                    Text = c.CustomerName,
                    Selected = c.CustomerId == model.CustomerId
                })
                .ToList();

            model.Products = _productService
                .GetActiveProducts()
                .ToList();
        }
    }
}