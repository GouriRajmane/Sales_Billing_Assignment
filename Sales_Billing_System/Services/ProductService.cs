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

        // Get all products
        public List<Product_Master> GetAllProducts()
        {
            return _productRepository.GetAllProducts();
        }

        // Get product by ID
        public Product_Master GetProductById(int productId)
        {
            return _productRepository.GetProductById(productId);
        }

        // Add new product
        public void AddProduct(Product_Master product)
        {
            if (product == null)
            {
                throw new ArgumentNullException("product");
            }

            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;
            product.IsActive = true;

            _productRepository.AddProduct(product);
        }

        // Update product
        public void UpdateProduct(Product_Master product)
        {
            if (product == null)
            {
                throw new ArgumentNullException("product");
            }

            product.UpdatedAt = DateTime.Now;

            _productRepository.UpdateProduct(product);
        }

        // Activate / Deactivate product
        public void ToggleStatus(int productId)
        {
            Product_Master product = _productRepository.GetProductById(productId);

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

        // Search products
        public List<Product_Master> SearchProduct(string searchText)
        {
            return _productRepository.SearchProduct(searchText);
        }
    }
}