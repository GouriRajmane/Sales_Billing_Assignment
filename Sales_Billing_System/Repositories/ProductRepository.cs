using MySql.Data.MySqlClient;
using Sales_Billing_System.Data;
using Sales_Billing_System.Models;
using Sales_Billing_System.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
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

        // Get List of all products ordered by ProductId descending
        public List<Product_Master> GetAllProducts()
        {
            return _context.Products
                            .OrderBy(p => p.ProductName)
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

        public List<Product_Master> GetActiveProducts()
        {
            return _context.Products
                           .Where(p => p.IsActive)
                           .OrderBy(p => p.ProductName)
                           .ToList();
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

        public PagedResult<Product_Master> GetProductsPaged(
    int pageNumber,
    int pageSize,
    string searchText)
        {
            var result = new PagedResult<Product_Master>();

            result.CurrentPage = pageNumber;
            result.PageSize = pageSize;

            using (var connection =
                (MySqlConnection)_context.Database.Connection)
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using (var command =
                    new MySqlCommand("sp_GetProductsPaged", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue(
                        "@p_PageNumber",
                        pageNumber);

                    command.Parameters.AddWithValue(
                        "@p_PageSize",
                        pageSize);

                    command.Parameters.AddWithValue(
                        "@p_SearchText",
                        string.IsNullOrWhiteSpace(searchText)
                            ? (object)DBNull.Value
                            : searchText.Trim());

                    using (var reader = command.ExecuteReader())
                    {
                        // ==============================
                        // RESULT SET 1 - PRODUCTS
                        // ==============================

                        while (reader.Read())
                        {
                            var product = new Product_Master
                            {
                                ProductId = Convert.ToInt32(
                                    reader["ProductId"]),

                                ProductName = reader["ProductName"] == DBNull.Value
                                    ? null
                                    : reader["ProductName"].ToString(),

                                SKU = reader["SKU"] == DBNull.Value
                                    ? null
                                    : reader["SKU"].ToString(),

                                Unit = reader["Unit"] == DBNull.Value
                                    ? null
                                    : reader["Unit"].ToString(),

                                SellingPrice = reader["SellingPrice"] == DBNull.Value
                                    ? 0
                                    : Convert.ToDecimal(
                                        reader["SellingPrice"]),

                                GSTPercentage = reader["GSTPercentage"] == DBNull.Value
                                    ? 0
                                    : Convert.ToDecimal(
                                        reader["GSTPercentage"]),

                                IsActive = reader["IsActive"] != DBNull.Value &&
                                           Convert.ToBoolean(
                                               reader["IsActive"])
                            };

                            result.Items.Add(product);
                        }

                        // ==============================
                        // RESULT SET 2 - TOTAL COUNT
                        // ==============================

                        if (reader.NextResult() && reader.Read())
                        {
                            result.TotalRecords =
                                Convert.ToInt32(
                                    reader["TotalRecords"]);
                        }
                    }
                }
            }

            return result;
        }


    }
}