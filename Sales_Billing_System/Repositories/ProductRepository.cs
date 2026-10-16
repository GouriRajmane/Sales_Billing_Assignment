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
        private const int MaxDropdownResults = 20;

        private readonly SalesBillingDbContext _context;

        public ProductRepository()
        {
            _context = new SalesBillingDbContext();
        }

        // Get all products with category and subcategory information.
        public List<Product_Master> GetAllProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .OrderBy(p => p.ProductId)
                .ToList();
        }

        // Get a product by ID with category and subcategory information.
        public Product_Master GetProductById(int productId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .FirstOrDefault(p => p.ProductId == productId);
        }

        // Search active categories by name (lazy-load dropdown).
        public List<Category_Master> SearchCategories(string term)
        {
            return _context.Categories
                .Where(c => c.IsActive &&
                            c.CategoryName.Contains(term))
                .OrderBy(c => c.CategoryName)
                .Take(MaxDropdownResults)
                .ToList();
        }

        // Search active subcategories of one category by name (lazy-load dropdown).
        public List<SubCategory_Master> SearchSubCategories(
            int categoryId,
            string term)
        {
            return _context.SubCategories
                .Where(sc =>
                    sc.IsActive &&
                    sc.CategoryId == categoryId &&
                    sc.Category.IsActive &&
                    sc.SubCategoryName.Contains(term))
                .OrderBy(sc => sc.SubCategoryName)
                .Take(MaxDropdownResults)
                .ToList();
        }

        // Get one category by ID.
        public Category_Master GetCategoryById(int categoryId)
        {
            return _context.Categories
                .FirstOrDefault(c => c.CategoryId == categoryId);
        }

        // Get one subcategory by ID.
        public SubCategory_Master GetSubCategoryById(int subCategoryId)
        {
            return _context.SubCategories
                .FirstOrDefault(sc => sc.SubCategoryId == subCategoryId);
        }

        // Add a new product.
        public void AddProduct(Product_Master product)
        {
            if (product == null)
            {
                throw new ArgumentNullException("product");
            }

            _context.Products.Add(product);
            _context.SaveChanges();
        }

        // Update an existing product.
        public void UpdateProduct(Product_Master product)
        {
            if (product == null)
            {
                throw new ArgumentNullException("product");
            }

            Product_Master existingProduct = _context.Products
                .FirstOrDefault(p => p.ProductId == product.ProductId);

            if (existingProduct == null)
            {
                throw new Exception("Product not found.");
            }

            existingProduct.CategoryId = product.CategoryId;
            existingProduct.SubCategoryId = product.SubCategoryId;
            existingProduct.ProductName = product.ProductName;
            existingProduct.SKU = product.SKU;
            existingProduct.Unit = product.Unit;
            existingProduct.SellingPrice = product.SellingPrice;
            existingProduct.GSTPercentage = product.GSTPercentage;
            existingProduct.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
        }

        // Activate or deactivate a product.
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

        // Get active products with their category information.
        public List<Product_Master> GetActiveProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToList();
        }

        // Search products by name, SKU, category, or subcategory.
        public List<Product_Master> SearchProduct(string searchText)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim();

                query = query.Where(p =>
                    p.ProductName.Contains(searchText) ||
                    (p.SKU != null && p.SKU.Contains(searchText)) ||
                    (p.Category != null &&
                     p.Category.CategoryName.Contains(searchText)) ||
                    (p.SubCategory != null &&
                     p.SubCategory.SubCategoryName.Contains(searchText)));
            }

            return query
                .OrderByDescending(p => p.ProductId)
                .ToList();
        }

        public PagedResult<Product_Master> GetProductsCursor(int pageSize, string searchText, int? lastId)
        {
            var result = new PagedResult<Product_Master> { PageSize = pageSize };
            var connection = (MySqlConnection)_context.Database.Connection;
            bool shouldClose = connection.State != ConnectionState.Open;
            try
            {
                if (shouldClose) connection.Open();
                using (var command = new MySqlCommand("sp_GetProductsCursor", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_PageSize", pageSize);
                    command.Parameters.AddWithValue("@p_SearchText", string.IsNullOrWhiteSpace(searchText) ? (object)DBNull.Value : searchText.Trim());
                    command.Parameters.AddWithValue("@p_LastId", lastId.HasValue ? (object)lastId.Value : DBNull.Value);

                    using (var reader = command.ExecuteReader())
                    {
                        Func<string, string> text = n => reader[n] == DBNull.Value ? null : Convert.ToString(reader[n]);

                        while (reader.Read())
                        {
                            var p = new Product_Master
                            {
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                ProductName = text("ProductName"),
                                SKU = text("SKU"),
                                Unit = text("Unit"),
                                SellingPrice = Convert.ToDecimal(reader["SellingPrice"]),
                                GSTPercentage = Convert.ToDecimal(reader["GSTPercentage"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CategoryId = Convert.ToInt32(reader["CategoryId"]),
                                SubCategoryId = reader["SubCategoryId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SubCategoryId"])
                            };
                            if (text("CategoryName") != null)
                                p.Category = new Category_Master { CategoryId = p.CategoryId, CategoryName = text("CategoryName") };
                            if (text("SubCategoryName") != null)
                                p.SubCategory = new SubCategory_Master { SubCategoryId = p.SubCategoryId, CategoryId = p.CategoryId, SubCategoryName = text("SubCategoryName") };
                            result.Items.Add(p);
                        }

                        if (reader.NextResult())
                            while (reader.Read())
                                result.GroupCounts[Convert.ToInt32(reader["CategoryId"])] = Convert.ToInt32(reader["ProductCount"]);
                    }
                }
            }
            finally { if (shouldClose && connection.State == ConnectionState.Open) connection.Close(); }

            result.TotalRecords = result.GroupCounts.Values.Sum();
            if (result.Items.Count > pageSize) { result.Items.RemoveAt(result.Items.Count - 1); result.HasMore = true; }
            return result;
        }
        // Safely locate a column without assuming it exists in the result set.
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