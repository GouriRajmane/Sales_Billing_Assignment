using Sales_Billing_System.Data;
using Sales_Billing_System.Models;
using Sales_Billing_System.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Sales_Billing_System.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly SalesBillingDbContext _context;

        public ProductRepository()
        {
            _context = new SalesBillingDbContext();
        }

        // Get List of all products
        public List<Product_Master> GetAllProducts()
        { 
            return _context.Products
                            .OrderByDescending(p => p.ProductId)
                            .ToList();
        }

        //Get product by ID
        public Product_Master GetProductById(int productId)
        {
            return _context.Products
                            .FirstOrDefault(p => p.ProductId == productId);
        }

        // Add a new product
        public void AddProduct(Product_Master product)
        { 
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        // Update existing product
        public void UpdateProduct(Product_Master product)
        {
            Product_Master existingProduct = GetProductById(product.ProductId);

            if (existingProduct != null)
            {
                existingProduct.ProductName = product.ProductName;
                existingProduct.SKU = product.SKU;
                existingProduct.Unit = product.Unit;
                existingProduct.SellingPrice = product.SellingPrice;
                existingProduct.GSTPercentage = product.GSTPercentage;
                //existingProduct.IsActive = product.IsActive;
                existingProduct.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
            }
        }

        public void ToggleStatus(int productId)
        {
            Product_Master product = GetProductById(productId);

            if (product != null)
            {
                product.IsActive = !product.IsActive;
                product.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
            }
        }

        // Search products
        public List<Product_Master> SearchProduct(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            { 
                return GetAllProducts();
            }

            return _context.Products
                            .Where(p =>
                            p.ProductName.Contains(searchText) || 
                            (p.SKU!= null && p.SKU.Contains(searchText)))
                            .OrderByDescending(p => p.ProductId)
                            .ToList();
        }


    }
}