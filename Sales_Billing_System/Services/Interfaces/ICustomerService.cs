using Sales_Billing_System.Models;
using System.Collections.Generic;

namespace Sales_Billing_System.Services.Interfaces
{
    public interface ICustomerService
    {
        List<Customer_Master> GetAllCustomers();

        Customer_Master GetCustomerById(int customerId);

        void AddCustomer(Customer_Master customer);

        void UpdateCustomer(Customer_Master customer);

        List<Customer_Master> SearchCustomers(string searchText);
    }
}