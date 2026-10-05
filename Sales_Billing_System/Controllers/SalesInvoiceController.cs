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


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public SalesInvoiceController()
        {
            _invoiceService =
                new SalesInvoiceService();

            _customerService =
                new CustomerService();

            _productService =
                new ProductService();
        }


        // =========================================================
        // INDEX
        // =========================================================

        public ActionResult Index(
            string searchText,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 10)
        {
            ViewBag.SearchText =
                searchText;

            ViewBag.FromDate =
                fromDate;

            ViewBag.ToDate =
                toDate;

            ViewBag.PageSize =
                pageSize;

            var invoices =
                _invoiceService.GetInvoicesPaged(
                    page,
                    pageSize,
                    searchText,
                    fromDate,
                    toDate);

            return View(invoices);
        }

        //public ActionResult Index(
        //    string searchText,
        //    DateTime? fromDate,
        //    DateTime? toDate)
        //{
        //    ViewBag.SearchText =
        //        searchText;

        //    ViewBag.FromDate =
        //        fromDate;

        //    ViewBag.ToDate =
        //        toDate;

        //    var invoices =
        //        _invoiceService.SearchInvoices(
        //            searchText,
        //            fromDate,
        //            toDate
        //        );

        //    return View(invoices);
        //}



        // =========================================================
        // CREATE - GET
        // =========================================================

        [HttpGet]
        public ActionResult Create()
        {
            SalesInvoiceViewModel model =
                new SalesInvoiceViewModel();


            // Generate invoice number

            model.InvoiceNumber =
                _invoiceService
                    .GenerateInvoiceNumber();


            // Add first invoice item

            model.Items.Add(
                new Sales_Invoice_Item()
            );


            // Load customers and products

            LoadDropdownData(model);


            return View(model);
        }


        // =========================================================
        // CREATE - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            SalesInvoiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdownData(model);

                return View(model);
            }


            try
            {
                _invoiceService
                    .CreateInvoice(model);


                TempData["SuccessMessage"] =
                    "Invoice created successfully.";


                return RedirectToAction(
                    "Index"
                );
            }
            catch (Exception ex)
            {
                LoadDropdownData(model);


                ModelState.AddModelError(
                    "",
                    ex.Message
                );


                return View(model);
            }
        }


        // =========================================================
        // DETAILS
        // =========================================================

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


        // =========================================================
        // PRINT
        // =========================================================

        public ActionResult Print(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(400, "Invalid invoice ID.");
            }

            Sales_Invoice invoice =
                _invoiceService.GetPrintInvoiceData(id);

            if (invoice == null)
            {
                return HttpNotFound("Invoice not found.");
            }

            return View(invoice);
        }

        //public ActionResult Print(int id)
        //{
        //    Sales_Invoice invoice =
        //        _invoiceService
        //            .GetInvoiceById(id);


        //    if (invoice == null)
        //    {
        //        return HttpNotFound();
        //    }


        //    return View(invoice);
        //}


        // =========================================================
        // CANCEL
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(int id)
        {
            try
            {
                _invoiceService
                    .CancelInvoice(id);


                TempData["SuccessMessage"] =
                    "Invoice cancelled successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    ex.Message;
            }


            return RedirectToAction(
                "Index"
            );
        }


        // =========================================================
        // GET PRODUCT DETAILS
        // =========================================================
        // This method is kept in case you want AJAX
        // product loading later.
        //
        // Current Create.cshtml does NOT depend on this method.
        // It uses data-rate and data-gst directly.
        // =========================================================

        [HttpGet]
        public JsonResult GetProductDetails(int id)
        {
            var product =
                _productService
                    .GetProductById(id);


            if (product == null ||
                !product.IsActive)
            {
                return Json(
                    new
                    {
                        success = false,
                        message =
                            "Product not found or inactive."
                    },
                    JsonRequestBehavior.AllowGet
                );
            }


            return Json(
                new
                {
                    success = true,

                    productId =
                        product.ProductId,

                    productName =
                        product.ProductName,

                    rate =
                        product.SellingPrice,

                    gstPercentage =
                        product.GSTPercentage
                },
                JsonRequestBehavior.AllowGet
            );
        }


        // =========================================================
        // LOAD DROPDOWN DATA
        // =========================================================

        private void LoadDropdownData(
            SalesInvoiceViewModel model)
        {
            // -----------------------------------------------------
            // CUSTOMERS
            // -----------------------------------------------------

            model.Customers =
                _customerService
                    .GetAllCustomers()
                    .Select(c => new SelectListItem
                    {
                        Value =
                            c.CustomerId.ToString(),

                        Text =
                            c.CustomerName,

                        Selected =
                            c.CustomerId ==
                            model.CustomerId
                    })
                    .ToList();


            // -----------------------------------------------------
            // ACTIVE PRODUCTS
            // -----------------------------------------------------

            model.Products =
                _productService
                    .GetActiveProducts()
                    .ToList();
        }


    }
}