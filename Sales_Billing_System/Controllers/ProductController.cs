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
        // CATEGORY DROPDOWN
        // =====================================================

        private void PopulateCategories(int? selectedCategoryId = null)
        {
            var categories = _productService.GetActiveCategories();

            ViewBag.Categories = new SelectList(
                categories,
                "CategoryId",
                "CategoryName",
                selectedCategoryId);
        }

        // =====================================================
        // SUBCATEGORY DROPDOWN
        // =====================================================

        private void PopulateSubCategories(
            int? categoryId = null,
            int? selectedSubCategoryId = null)
        {
            // Always provide a list so the Razor view never
            // receives a null ViewBag.SubCategories value.
            var subCategories = Enumerable.Empty<SubCategory_Master>();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                subCategories =
                    _productService.GetActiveSubCategoriesByCategoryId(
                        categoryId.Value);
            }

            // Only select a subcategory belonging to this category.
            if (!selectedSubCategoryId.HasValue ||
                !subCategories.Any(s =>
                    s.SubCategoryId == selectedSubCategoryId.Value))
            {
                selectedSubCategoryId = null;
            }

            ViewBag.SubCategories = new SelectList(
                subCategories,
                "SubCategoryId",
                "SubCategoryName",
                selectedSubCategoryId);
        }

        // =====================================================
        // UNIT DROPDOWN
        // =====================================================

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

            ViewBag.Units = new SelectList(units, selectedUnit);
        }

        // Populate all dropdowns whenever a form is displayed.
        private void PopulateProductDropdowns(
            int? categoryId = null,
            int? subCategoryId = null,
            string selectedUnit = null)
        {
            PopulateCategories(categoryId);
            PopulateSubCategories(categoryId, subCategoryId);
            PopulateUnits(selectedUnit);
        }

        // =====================================================
        // AJAX: GET SUBCATEGORIES BY CATEGORY
        // =====================================================

        [HttpGet]
        public JsonResult GetSubCategoriesByCategory(int categoryId)
        {
            if (categoryId <= 0)
            {
                return Json(
                    new object[0],
                    JsonRequestBehavior.AllowGet);
            }

            var subCategories =
                _productService.GetActiveSubCategoriesByCategoryId(
                    categoryId);

            var result = subCategories
                .Select(s => new
                {
                    Value = s.SubCategoryId,
                    Text = s.SubCategoryName
                })
                .ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // =====================================================
        // CREATE PRODUCT - GET
        // =====================================================

        [HttpGet]
        public ActionResult Create()
        {
            PopulateProductDropdowns();

            return PartialView(new Product_Master());
        }

        // =====================================================
        // CREATE PRODUCT - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product_Master product)
        {
            if (Convert.ToInt32(product.CategoryId) <= 0)
            {
                ModelState.AddModelError(
                    "CategoryId",
                    "Please select a category.");
            }

            if (Convert.ToInt32(product.SubCategoryId) <= 0)
            {
                ModelState.AddModelError(
                    "SubCategoryId",
                    "Please select a subcategory.");
            }

            if (!ModelState.IsValid)
            {
                PopulateProductDropdowns(
                    Convert.ToInt32(product.CategoryId),
                    Convert.ToInt32(product.SubCategoryId),
                    product.Unit);

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

                PopulateProductDropdowns(
                    Convert.ToInt32(product.CategoryId),
                    Convert.ToInt32(product.SubCategoryId),
                    product.Unit);

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

            PopulateProductDropdowns(
                Convert.ToInt32(product.CategoryId),
                Convert.ToInt32(product.SubCategoryId),
                product.Unit);

            return PartialView(product);
        }

        // =====================================================
        // EDIT PRODUCT - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product_Master product)
        {
            if (Convert.ToInt32(product.ProductId) <= 0)
            {
                ModelState.AddModelError(
                    "ProductId",
                    "Invalid product ID.");
            }

            if (Convert.ToInt32(product.CategoryId) <= 0)
            {
                ModelState.AddModelError(
                    "CategoryId",
                    "Please select a category.");
            }

            if (Convert.ToInt32(product.SubCategoryId) <= 0)
            {
                ModelState.AddModelError(
                    "SubCategoryId",
                    "Please select a subcategory.");
            }

            if (!ModelState.IsValid)
            {
                PopulateProductDropdowns(
                    Convert.ToInt32(product.CategoryId),
                    Convert.ToInt32(product.SubCategoryId),
                    product.Unit);

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

                PopulateProductDropdowns(
                    Convert.ToInt32(product.CategoryId),
                    Convert.ToInt32(product.SubCategoryId),
                    product.Unit);

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
    }
}
