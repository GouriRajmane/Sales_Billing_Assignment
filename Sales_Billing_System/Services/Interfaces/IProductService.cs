using Sales_Billing_System.Models;
using System.Collections.Generic;

namespace Sales_Billing_System.Services.Interfaces
{
    public interface IProductService
    {
        List<Product_Master> GetAllProducts();

        Product_Master GetProductById(int productId);

        void AddProduct(Product_Master product);

        void UpdateProduct(Product_Master product);

        void ToggleStatus(int productId);

        List<Product_Master> GetActiveProducts();

        List<Product_Master> SearchProduct(string searchText);

        PagedResult<Product_Master> GetProductsPaged(
            int pageNumber,
            int pageSize,
            string searchText);

        PagedResult<Product_Master> GetProductsCursor(
            int pageSize,
            string searchText,
            int? lastId);


        // Lazy-load dropdown search
        List<Category_Master> SearchCategories(string term);

        List<SubCategory_Master> SearchSubCategories(
            int categoryId,
            string term);

        // Lookups used to show the selected item
        Category_Master GetCategoryById(int categoryId);

        SubCategory_Master GetSubCategoryById(int subCategoryId);
    }
}