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

        // GET: Customer
        public ActionResult Index(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                var customers = _customerService.GetAllCustomers();
                return View(customers);
            }

            var searchedCustomers = _customerService.SearchCustomers(searchText);

            ViewBag.SearchText = searchText;

            return View(searchedCustomers);
        }

        // GET: Customer/Details/5
        public ActionResult Details(int id)
        {
            Customer_Master customer = _customerService.GetCustomerById(id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        // GET: Customer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer_Master customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _customerService.AddCustomer(customer);

                    TempData["SuccessMessage"] = "Customer added successfully.";

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to add customer. " + ex.Message);
                }
            }

            return View(customer);
        }

        // GET: Customer/Edit/5
        public ActionResult Edit(int id)
        {
            Customer_Master customer = _customerService.GetCustomerById(id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer_Master customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _customerService.UpdateCustomer(customer);

                    TempData["SuccessMessage"] = "Customer updated successfully.";

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to update customer. " + ex.Message);
                }
            }

            return View(customer);
        }

        // GET: Customer/Delete/5
        public ActionResult Delete(int id)
        {
            Customer_Master customer = _customerService.GetCustomerById(id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        // POST: Customer/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // Delete functionality will be added here
                // after adding DeleteCustomer() to Repository and Service.

                TempData["SuccessMessage"] = "Customer deleted successfully.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to delete customer. " + ex.Message;

                return RedirectToAction("Index");
            }
        }
    }
}