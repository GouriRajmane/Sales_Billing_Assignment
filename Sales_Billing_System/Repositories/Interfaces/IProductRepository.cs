using Sales_Billing_System.Models;
using System.Collections.Generic;

namespace Sales_Billing_System.Repositories.Interfaces
{
    public interface IProductRepository
    {
        // Get all products
        List<Product_Master> GetAllProducts();

        // Get a single product by ID
        Product_Master GetProductById(int productId);

        // Add a new product
        void AddProduct(Product_Master product);

        // Update an existing product
        void UpdateProduct(Product_Master product);

        // Activate or Deactivate product
        void ToggleStatus(int productId);

        List<Product_Master> GetActiveProducts();

        // Search products
        List<Product_Master> SearchProduct(string searchText);
    }
}