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

        PagedResult<Product_Master> GetProductsPaged(
            int pageNumber,
            int pageSize,
            string searchText);

        List<Category_Master> GetActiveCategories();

        List<SubCategory_Master> GetActiveSubCategoriesByCategoryId(
            int categoryId);
    }
}