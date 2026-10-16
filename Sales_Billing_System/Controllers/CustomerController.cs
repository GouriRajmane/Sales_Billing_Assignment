using Sales_Billing_System.Models;
using Sales_Billing_System.Services;
using Sales_Billing_System.Services.Interfaces;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Sales_Billing_System.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController()
        {
            _customerService = new CustomerService();
        }

        // Customer Listing and Search

        // GET: Customer
        public ActionResult Index(string searchText)
        {
            ViewBag.SearchText = searchText;
            return View();   // rows now arrive through LoadCustomers
        }

        // Customer Details - loads into modal
        [HttpGet]
        public ActionResult Details(int id)
        {
            Customer_Master customer =
                _customerService.GetCustomerById(id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return PartialView(customer);
        }

        // Create Customer - GET (loads into modal)
        [HttpGet]
        public ActionResult Create()
        {
            return PartialView(new Customer_Master());
        }

        // Create Customer - POST (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer_Master customer)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(customer);
            }

            try
            {
                _customerService.AddCustomer(customer);

                return Json(new
                {
                    success = true,
                    message = "Customer added successfully."
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to add customer. " + ex.Message
                );

                return PartialView(customer);
            }
        }

        // Edit Customer - GET (loads into modal)
        [HttpGet]
        public ActionResult Edit(int id)
        {
            Customer_Master customer =
                _customerService.GetCustomerById(id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return PartialView(customer);
        }

        // Edit Customer - POST (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer_Master customer)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(customer);
            }

            try
            {
                _customerService.UpdateCustomer(customer);

                return Json(new
                {
                    success = true,
                    message = "Customer updated successfully."
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to update customer. " + ex.Message
                );

                return PartialView(customer);
            }
        }

        [HttpGet]
        public JsonResult LoadCustomers(string searchText, int? lastId, int pageSize = 20)
        {
            var r = _customerService.GetCustomersCursor(pageSize, searchText, lastId);

            var items = r.Items.Select(c => new
            {
                id = c.CustomerId,
                name = c.CustomerName,
                gstin = c.GSTIN,
                mobile = c.MobileNumber,
                address = c.Address,
                detailsUrl = Url.Action("Details", "Customer", new { id = c.CustomerId }),
                editUrl = Url.Action("Edit", "Customer", new { id = c.CustomerId })
            });

            return Json(new { items, hasMore = r.HasMore, total = r.TotalRecords }, JsonRequestBehavior.AllowGet);
        }
    }
}