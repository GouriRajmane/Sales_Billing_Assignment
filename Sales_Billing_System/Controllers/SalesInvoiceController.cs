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
        private readonly ISalesInvoiceService _invoiceService = new SalesInvoiceService();

        // INDEX
        public ActionResult Index(string searchText, DateTime? fromDate, DateTime? toDate)
        {
            ViewBag.SearchText = searchText;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return View();   // rows now arrive through LoadInvoices
        }

        // CREATE - GET (loads into modal)
        [HttpGet]
        public ActionResult Create()
        {
            var model = new SalesInvoiceViewModel { InvoiceNumber = _invoiceService.GenerateInvoiceNumber() };
            model.Items.Add(new Sales_Invoice_Item());
            return PartialView(model);
        }

        // CREATE - POST (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SalesInvoiceViewModel model)
        {
            if (!ModelState.IsValid) return PartialView(model);

            try
            {
                _invoiceService.CreateInvoice(model);
                return Json(new { success = true, message = "Invoice created successfully." });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return PartialView(model);
            }
        }

        // DETAILS (modal)
        [HttpGet]
        public ActionResult Details(int id)
        {
            var invoice = _invoiceService.GetInvoiceById(id);
            return invoice == null ? (ActionResult)HttpNotFound() : PartialView(invoice);
        }

        // PRINT (full page, new tab)
        public ActionResult Print(int id)
        {
            if (id <= 0) return new HttpStatusCodeResult(400, "Invalid invoice ID.");

            var invoice = _invoiceService.GetPrintInvoiceData(id);
            return invoice == null ? (ActionResult)HttpNotFound("Invoice not found.") : View(invoice);
        }

        // CANCEL - confirmation modal
        [HttpGet]
        public ActionResult ConfirmCancel(int id)
        {
            var invoice = _invoiceService.GetInvoiceById(id);
            return invoice == null ? (ActionResult)HttpNotFound() : PartialView(invoice);
        }

        // CANCEL - POST (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(int id)
        {
            try
            {
                _invoiceService.CancelInvoice(id);
                return Json(new { success = true, message = "Invoice cancelled successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // LAZY SEARCH - CUSTOMERS  -> [{ id, text }]
        [HttpGet]
        public JsonResult SearchCustomers(string term)
        {
            var data = _invoiceService.SearchCustomers(term)
                .Select(c => new { id = c.CustomerId, text = c.CustomerName + " (" + c.MobileNumber + ")" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // LAZY SEARCH - PRODUCTS  -> [{ id, text, rate, gst }]
        [HttpGet]
        public JsonResult SearchProducts(string term)
        {
            var data = _invoiceService.SearchProducts(term)
                .Select(p => new { id = p.ProductId, text = p.ProductName, rate = p.SellingPrice, gst = p.GSTPercentage });

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult LoadInvoices(string searchText, DateTime? fromDate, DateTime? toDate, DateTime? lastDate, int? lastId, int pageSize = 20)
        {
            var r = _invoiceService.GetInvoicesCursor(pageSize, searchText, fromDate, toDate, lastDate, lastId);

            var items = r.Items.Select(i => new
            {
                id = i.InvoiceId,
                invoiceNumber = i.InvoiceNumber,
                date = i.InvoiceDate.ToString("dd-MM-yyyy"),
                cursorDate = i.InvoiceDate.ToString("yyyy-MM-dd HH:mm:ss"),
                customer = i.Customer != null ? i.Customer.CustomerName : "",
                total = i.GrandTotal.ToString("N2"),
                status = i.Status,
                detailsUrl = Url.Action("Details", "SalesInvoice", new { id = i.InvoiceId }),
                printUrl = Url.Action("Print", "SalesInvoice", new { id = i.InvoiceId }),
                cancelUrl = Url.Action("ConfirmCancel", "SalesInvoice", new { id = i.InvoiceId })
            });

            return Json(new { items, hasMore = r.HasMore, total = r.TotalRecords }, JsonRequestBehavior.AllowGet);
        }
    }
}