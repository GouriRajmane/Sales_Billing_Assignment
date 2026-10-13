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

        // Fixed list of units, searched as the user types.
        private static readonly string[] Units =
        {
            "Plate", "Bowl", "Piece", "Cup", "Glass", "Scoop", "Serving",
            "Kg", "Gram", "Litre", "Millilitre", "Packet", "Dozen"
        };

        public ProductController()
        {
            _productService = new ProductService();
        }

        // =====================================================
        // PRODUCT LIST
        // =====================================================

        [HttpGet]
        public ActionResult Index(
            string searchText,
            int page = 1,
            int pageSize = 10)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 10;

            var result = _productService.GetProductsPaged(
                page,
                pageSize,
                searchText);

            ViewBag.SearchText = searchText;
            ViewBag.PageSize = pageSize;

            return View(result);
        }



        // =====================================================
        // LAZY-LOAD DROPDOWN SEARCH (returns { results: [{id, text}] })
        // =====================================================

        [HttpGet]
        public JsonResult SearchCategories(string term)
        {
            var results = _productService
                .SearchCategories(term)
                .Select(c => new { id = c.CategoryId, text = c.CategoryName })
                .ToList();

            return Json(new { results }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchSubCategories(int categoryId, string term)
        {
            var results = _productService
                .SearchSubCategories(categoryId, term)
                .Select(s => new { id = s.SubCategoryId, text = s.SubCategoryName })
                .ToList();

            return Json(new { results }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchUnits(string term)
        {
            term = (term ?? "").Trim();

            var results = term.Length == 0
                ? new object[0]
                : Units
                    .Where(u => u.IndexOf(
                        term, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Select(u => (object)new { id = u, text = u })
                    .ToArray();

            return Json(new { results }, JsonRequestBehavior.AllowGet);
        }

        // =====================================================
        // CREATE PRODUCT - GET
        // =====================================================

        [HttpGet]
        public ActionResult Create()
        {
            return PartialView(new Product_Master());
        }

        // =====================================================
        // CREATE PRODUCT - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product_Master product)
        {
            ValidateDropdowns(product);

            if (!ModelState.IsValid)
            {
                LoadSelectedNames(product);
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

                LoadSelectedNames(product);
                return PartialView(product);
            }
        }

        // =====================================================
        // PRODUCT DETAILS - GET
        // =====================================================

        [HttpGet]
        public ActionResult Details(int id)
        {
            var product = _productService.GetProductById(id);

            if (product == null)
                return HttpNotFound();

            return PartialView(product);
        }

        // =====================================================
        // EDIT PRODUCT - GET
        // =====================================================

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var product = _productService.GetProductById(id);

            if (product == null)
                return HttpNotFound();

            LoadSelectedNames(product);

            return PartialView(product);
        }

        // =====================================================
        // EDIT PRODUCT - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product_Master product)
        {
            if (product.ProductId <= 0)
            {
                ModelState.AddModelError("ProductId", "Invalid product ID.");
            }

            ValidateDropdowns(product);

            if (!ModelState.IsValid)
            {
                LoadSelectedNames(product);
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

                LoadSelectedNames(product);
                return PartialView(product);
            }
        }

        // =====================================================
        // ACTIVATE / DEACTIVATE - GET
        // =====================================================

        [HttpGet]
        public ActionResult ConfirmToggleStatus(int id)
        {
            var product = _productService.GetProductById(id);

            if (product == null)
                return HttpNotFound();

            return PartialView(product);
        }

        // =====================================================
        // ACTIVATE / DEACTIVATE - POST
        // =====================================================

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

        // =====================================================
        // HELPERS
        // =====================================================

        // Category, subcategory and unit must be chosen from the search results.
        private void ValidateDropdowns(Product_Master product)
        {
            if (product.CategoryId <= 0)
            {
                ModelState.AddModelError(
                    "CategoryId", "Please select a category.");
            }

            if (product.SubCategoryId <= 0)
            {
                ModelState.AddModelError(
                    "SubCategoryId", "Please select a subcategory.");
            }

            if (string.IsNullOrWhiteSpace(product.Unit) ||
                !Units.Contains(product.Unit))
            {
                ModelState.AddModelError(
                    "Unit", "Please select a unit.");
            }
        }

        // The dropdowns start empty, so the form needs the selected names
        // to show the current choice (Edit, or Create/Edit after an error).
        private void LoadSelectedNames(Product_Master product)
        {
            var category = product.CategoryId > 0
                ? _productService.GetCategoryById(product.CategoryId)
                : null;

            var subCategory = product.SubCategoryId > 0
                ? _productService.GetSubCategoryById(product.SubCategoryId)
                : null;

            ViewBag.SelectedCategoryName =
                category != null ? category.CategoryName : null;

            ViewBag.SelectedSubCategoryName =
                subCategory != null ? subCategory.SubCategoryName : null;
        }
    }
}