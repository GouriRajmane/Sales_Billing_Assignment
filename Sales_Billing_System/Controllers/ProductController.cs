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
        public ActionResult Index(
            string searchText,
            int page = 1,
            int pageSize = 10)
        {
            var result = _productService.GetProductsPaged(
                page,
                pageSize,
                searchText);

            ViewBag.SearchText = searchText;
            ViewBag.PageSize = pageSize;

            return View(result);
        }

        // Create Product - GET (loads into modal)
        [HttpGet]
        public ActionResult Create()
        {
            return PartialView(new Product_Master());
        }

        // Create Product - POST (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product_Master product)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(product);
            }

            try
            {
                _productService.AddProduct(product);

                return Json(new
                {
                    success = true,
                    message = "Product added successfully."
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                return PartialView(product);
            }
        }

        // Edit Product - GET (loads into modal)
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var product = _productService.GetProductById(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return PartialView(product);
        }

        // Edit Product - POST (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product_Master product)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(product);
            }

            try
            {
                _productService.UpdateProduct(product);

                return Json(new
                {
                    success = true,
                    message = "Product updated successfully."
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                return PartialView(product);
            }
        }

        // Activate / Deactivate - confirmation modal (GET)
        [HttpGet]
        public ActionResult ConfirmToggleStatus(int id)
        {
            var product = _productService.GetProductById(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return PartialView(product);
        }

        // Activate / Deactivate - POST (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleStatus(int id)
        {
            try
            {
                _productService.ToggleStatus(id);

                return Json(new
                {
                    success = true,
                    message = "Product status updated successfully."
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
    }
}