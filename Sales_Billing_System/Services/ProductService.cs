using Sales_Billing_System.Models;
using Sales_Billing_System.Repositories;
using Sales_Billing_System.Repositories.Interfaces;
using Sales_Billing_System.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public List<Category_Master> GetActiveCategories()
        {
            return _productRepository.GetActiveCategories();
        }

        public void AddProduct(Product_Master product)
        {
            if (product == null)
            {
                throw new ArgumentNullException("product");
            }

            ValidateCategory(product.CategoryId);

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

            ValidateCategory(product.CategoryId);

            product.UpdatedAt = DateTime.Now;

            _productRepository.UpdateProduct(product);
        }

        public void ToggleStatus(int productId)
        {
            Product_Master product =
                _productRepository.GetProductById(productId);

            if (product == null)
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

        private void ValidateCategory(int categoryId)
        {
            if (categoryId <= 0)
            {
                throw new Exception("Please select a category.");
            }

            bool categoryExists = _productRepository
                .GetActiveCategories()
                .Any(c => c.CategoryId == categoryId);

            if (!categoryExists)
            {
                throw new Exception(
                    "The selected category does not exist or is inactive.");
            }
        }
    }
}