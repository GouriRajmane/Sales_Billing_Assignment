using Sales_Billing_System.Models;
using Sales_Billing_System.Services;
using Sales_Billing_System.Services.Interfaces;
using System;
using System.Linq;
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

        // Product listing
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

        // Load category dropdown
        private void PopulateCategories(int? selectedCategoryId = null)
        {
            var categories = _productService.GetActiveCategories();

            ViewBag.Categories = new SelectList(
                categories,
                "CategoryId",
                "CategoryName",
                selectedCategoryId);
        }


        private void PopulateUnits(string selectedUnit = null)
        {
            var units = new[]
            {
                "Plate",
                "Bowl",
                "Piece",
                "Cup",
                "Glass",
                "Scoop",
                "Serving",
                "Kg",
                "Gram",
                "Litre",
                "Millilitre",
                "Packet",
                "Dozen"
            };

            ViewBag.Units = new SelectList(
                units,
                selectedUnit
            );
        }

        // Create product - GET
        [HttpGet]
        public ActionResult Create()
        {
            PopulateCategories();
            PopulateUnits();

            return PartialView(new Product_Master());
        }

        // Create product - POST (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product_Master product)
        {
            if (product.CategoryId <= 0)
            {
                ModelState.AddModelError(
                    "CategoryId",
                    "Please select a category.");
            }

            if (!ModelState.IsValid)
            {
                PopulateCategories(product.CategoryId);
                PopulateUnits(product.Unit);

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

                PopulateCategories(product.CategoryId);
                PopulateUnits(product.Unit);

                return PartialView(product);
            }
        }

        // Edit product - GET
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var product = _productService.GetProductById(id);

            if (product == null)
                return HttpNotFound();

            PopulateCategories(product.CategoryId);
            PopulateUnits(product.Unit);

            return PartialView(product);
        }

        // Edit product - POST (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product_Master product)
        {
            if (product.CategoryId <= 0)
            {
                ModelState.AddModelError(
                    "CategoryId",
                    "Please select a category.");
            }

            if (!ModelState.IsValid)
            {
                PopulateCategories(product.CategoryId);
                PopulateUnits(product.Unit);
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

                PopulateCategories(product.CategoryId);
                PopulateUnits(product.Unit);
                return PartialView(product);
            }
        }

        // Activate/deactivate confirmation modal - GET
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

        // Activate/deactivate - POST (AJAX)
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