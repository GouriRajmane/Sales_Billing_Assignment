using Sales_Billing_System.Models;
using Sales_Billing_System.Services;
using Sales_Billing_System.Services.Interfaces;
using System;
using System.Web.Mvc;

namespace Sales_Billing_System.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController()
        {
            _productService = new ProductService();
        }

        // Product Listing
        public ActionResult Index(string searchText)
        {
            var products = string.IsNullOrWhiteSpace(searchText)
                ? _productService.GetAllProducts()
                : _productService.SearchProduct(searchText);

            ViewBag.SearchText = searchText;

            return View(products);
        }

        // Create Product - GET
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // Create Product - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product_Master product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            try
            {
                _productService.AddProduct(product);

                TempData["SuccessMessage"] = "Product added successfully.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                return View(product);
            }
        }

        // Edit Product - GET
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var product = _productService.GetProductById(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // Edit Product - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product_Master product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            try
            {
                _productService.UpdateProduct(product);

                TempData["SuccessMessage"] = "Product updated successfully.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                return View(product);
            }
        }

        // Activate / Deactivate Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleStatus(int id)
        {
            try
            {
                _productService.ToggleStatus(id);

                TempData["SuccessMessage"] =
                    "Product status updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }


    }
}