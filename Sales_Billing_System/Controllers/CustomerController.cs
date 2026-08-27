using Sales_Billing_System.Models;
using Sales_Billing_System.Services;
using Sales_Billing_System.Services.Interfaces;
using System;
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
        public ActionResult Index(string searchText)
        {
            ViewBag.SearchText = searchText;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                return View(_customerService.GetAllCustomers());
            }

            return View(
                _customerService.SearchCustomers(searchText)
            );
        }

        // Customer Details
        public ActionResult Details(int id)
        {
            Customer_Master customer =
                _customerService.GetCustomerById(id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        // Create Customer - GET
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // Create Customer - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer_Master customer)
        {
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            try
            {
                _customerService.AddCustomer(customer);

                TempData["SuccessMessage"] =
                    "Customer added successfully.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to add customer. " + ex.Message
                );

                return View(customer);
            }
        }

        // Edit Customer - GET
        [HttpGet]
        public ActionResult Edit(int id)
        {
            Customer_Master customer =
                _customerService.GetCustomerById(id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        // Edit Customer - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer_Master customer)
        {
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            try
            {
                _customerService.UpdateCustomer(customer);

                TempData["SuccessMessage"] =
                    "Customer updated successfully.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to update customer. " + ex.Message
                );

                return View(customer);
            }
        }
    }
}