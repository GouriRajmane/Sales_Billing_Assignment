using Sales_Billing_System.Models;
using System.Collections.Generic;

namespace Sales_Billing_System.Repositories.Interfaces
{
    public interface IProductRepository
    {
        List<Product_Master> GetAllProducts();

        Product_Master GetProductById(int productId);

        void AddProduct(Product_Master product);

        void UpdateProduct(Product_Master product);

        void ToggleStatus(int productId);

        List<Product_Master> GetActiveProducts();

        List<Product_Master> SearchProduct(string searchText);

        PagedResult<Product_Master> GetProductsCursor(
            int pageSize,
            string searchText,
            int? lastId);

        // Lazy-load dropdown search
        List<Category_Master> SearchCategories(string term);

        List<SubCategory_Master> SearchSubCategories(
            int categoryId,
            string term);

        // Lookups used for validation and for showing the selected item
        Category_Master GetCategoryById(int categoryId);

        SubCategory_Master GetSubCategoryById(int subCategoryId);
    }
}