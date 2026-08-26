using Sales_Billing_System.Models;
using System.Collections.Generic;

namespace Sales_Billing_System.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        List<Customer_Master> GetAllCustomers();

        Customer_Master GetCustomerById(int customerId);

        void AddCustomer(Customer_Master customer);

        void UpdateCustomer(Customer_Master customer);

        void DeleteCustomer(int customerId);

        List<Customer_Master> SearchCustomers(string searchText);
    }
}