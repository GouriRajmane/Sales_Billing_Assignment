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

        // Get all products with category information
        public List<Product_Master> GetAllProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.ProductId)
                .ToList();
        }

        // Get product by ID with category information
        public Product_Master GetProductById(int productId)
        {
            return _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.ProductId == productId);
        }

        // Get active categories for dropdowns
        public List<Category_Master> GetActiveCategories()
        {
            return _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.CategoryId)
                .ToList();
        }

        // Add a new product
        public void AddProduct(Product_Master product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        // Update an existing product
        public void UpdateProduct(Product_Master product)
        {
            Product_Master existingProduct =
                _context.Products.FirstOrDefault(
                    p => p.ProductId == product.ProductId);

            if (existingProduct == null)
            {
                throw new Exception("Product not found.");
            }

            existingProduct.ProductName = product.ProductName;
            existingProduct.SKU = product.SKU;
            existingProduct.Unit = product.Unit;
            existingProduct.SellingPrice = product.SellingPrice;
            existingProduct.GSTPercentage = product.GSTPercentage;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
        }

        // Activate or deactivate a product
        public void ToggleStatus(int productId)
        {
            Product_Master product = _context.Products
                .FirstOrDefault(p => p.ProductId == productId);

            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            product.IsActive = !product.IsActive;
            product.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
        }

        // Get active products
        public List<Product_Master> GetActiveProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToList();
        }

        // Search products
        public List<Product_Master> SearchProduct(string searchText)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim();

                query = query.Where(p =>
                    p.ProductName.Contains(searchText) ||
                    (p.SKU != null && p.SKU.Contains(searchText)) ||
                    (p.Category != null && p.Category.CategoryName.Contains(searchText)));
            }

            return query
                .OrderByDescending(p => p.ProductId)
                .ToList();
        }

        // Pagination through MySQL stored procedure
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
                        "@p_PageNumber", pageNumber);

                    command.Parameters.AddWithValue(
                        "@p_PageSize", pageSize);

                    command.Parameters.AddWithValue(
                        "@p_SearchText",
                        string.IsNullOrWhiteSpace(searchText)
                            ? (object)DBNull.Value
                            : searchText.Trim());

                    using (var reader = command.ExecuteReader())
                    {
                        // Result set 1: Products
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
                                    Convert.ToBoolean(reader["IsActive"])
                            };

                            // These fields must be returned by the updated SP.
                            int categoryIdOrdinal =
                                GetOrdinalIfExists(reader, "CategoryId");

                            int categoryNameOrdinal =
                                GetOrdinalIfExists(reader, "CategoryName");

                            if (categoryIdOrdinal >= 0 &&
                                reader.IsDBNull(categoryIdOrdinal) == false)
                            {
                                product.CategoryId =
                                    Convert.ToInt32(reader.GetValue(
                                        categoryIdOrdinal));
                            }

                            if (categoryNameOrdinal >= 0 &&
                                reader.IsDBNull(categoryNameOrdinal) == false)
                            {
                                product.Category = new Category_Master
                                {
                                    CategoryId = product.CategoryId,
                                    CategoryName = reader.GetString(
                                        categoryNameOrdinal)
                                };
                            }

                            result.Items.Add(product);
                        }

                        // Result set 2: Total record count
                        if (reader.NextResult() && reader.Read())
                        {
                            result.TotalRecords = Convert.ToInt32(
                                reader["TotalRecords"]);
                        }
                    }
                }
            }

            return result;
        }

        private int GetOrdinalIfExists(
            IDataRecord reader,
            string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(
                    reader.GetName(i),
                    columnName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}