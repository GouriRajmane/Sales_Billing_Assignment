using Sales_Billing_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sales_Billing_System.Services.Interfaces
{
    public interface IProductService
    {
        List<Product_Master> GetAllProducts();

        Product_Master GetProductById(int productId);

        void AddProduct(Product_Master product);

        void UpdateProduct(Product_Master product);

        void ToggleStatus(int productId);

        List<Product_Master> SearchProduct(string searchText);
    }
}