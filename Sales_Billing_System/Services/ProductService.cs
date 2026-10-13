using Sales_Billing_System.Models;
using Sales_Billing_System.Repositories;
using Sales_Billing_System.Repositories.Interfaces;
using Sales_Billing_System.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Sales_Billing_System.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService()
        {
            _productRepository = new ProductRepository();
        }

        public List<Product_Master> GetAllProducts()
        {
            return _productRepository.GetAllProducts();
        }

        public Product_Master GetProductById(int productId)
        {
            return _productRepository.GetProductById(productId);
        }

        // Nothing is returned until the user types something.
        public List<Category_Master> SearchCategories(string term)
        {
            term = (term ?? "").Trim();

            if (term.Length == 0)
            {
                return new List<Category_Master>();
            }

            return _productRepository.SearchCategories(term);
        }

        // Nothing is returned until a category is chosen and the user types.
        public List<SubCategory_Master> SearchSubCategories(
            int categoryId,
            string term)
        {
            term = (term ?? "").Trim();

            if (categoryId <= 0 || term.Length == 0)
            {
                return new List<SubCategory_Master>();
            }

            return _productRepository.SearchSubCategories(categoryId, term);
        }

        public Category_Master GetCategoryById(int categoryId)
        {
            return _productRepository.GetCategoryById(categoryId);
        }

        public SubCategory_Master GetSubCategoryById(int subCategoryId)
        {
            return _productRepository.GetSubCategoryById(subCategoryId);
        }

        public void AddProduct(Product_Master product)
        {
            if (product == null)
            {
                throw new ArgumentNullException("product");
            }

            ValidateCategoryAndSubCategory(
                product.CategoryId,
                product.SubCategoryId);

            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;
            product.IsActive = true;

            _productRepository.AddProduct(product);
        }

        public void UpdateProduct(Product_Master product)
        {
            if (product == null)
            {
                throw new ArgumentNullException("product");
            }

            if (_productRepository.GetProductById(product.ProductId) == null)
            {
                throw new Exception("Product not found.");
            }

            ValidateCategoryAndSubCategory(
                product.CategoryId,
                product.SubCategoryId);

            product.UpdatedAt = DateTime.Now;

            _productRepository.UpdateProduct(product);
        }

        public void ToggleStatus(int productId)
        {
            if (_productRepository.GetProductById(productId) == null)
            {
                throw new Exception("Product not found.");
            }

            _productRepository.ToggleStatus(productId);
        }

        public List<Product_Master> GetActiveProducts()
        {
            return _productRepository.GetActiveProducts();
        }

        public List<Product_Master> SearchProduct(string searchText)
        {
            return _productRepository.SearchProduct(searchText);
        }

        public PagedResult<Product_Master> GetProductsPaged(
            int pageNumber,
            int pageSize,
            string searchText)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 10;
            }

            if (pageSize > 100)
            {
                pageSize = 100;
            }

            return _productRepository.GetProductsPaged(
                pageNumber,
                pageSize,
                searchText);
        }

        // Category and subcategory must exist, be active,
        // and the subcategory must belong to the category.
        private void ValidateCategoryAndSubCategory(
            int categoryId,
            int subCategoryId)
        {
            Category_Master category =
                categoryId > 0
                    ? _productRepository.GetCategoryById(categoryId)
                    : null;

            if (category == null || !category.IsActive)
            {
                throw new Exception(
                    "Please select a valid active category.");
            }

            SubCategory_Master subCategory =
                subCategoryId > 0
                    ? _productRepository.GetSubCategoryById(subCategoryId)
                    : null;

            if (subCategory == null ||
                !subCategory.IsActive ||
                subCategory.CategoryId != categoryId)
            {
                throw new Exception(
                    "Please select a valid active subcategory " +
                    "for the selected category.");
            }
        }
    }
}