using Sales_Billing_System.Models;
using Sales_Billing_System.Repositories;
using Sales_Billing_System.Repositories.Interfaces;
using Sales_Billing_System.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Sales_Billing_System.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService()
        {
            _customerRepository = new CustomerRepository();
        }

        // Get all customers
        public List<Customer_Master> GetAllCustomers()
        {
            return _customerRepository.GetAllCustomers();
        }

        // Get customer by ID
        public Customer_Master GetCustomerById(int customerId)
        {
            return _customerRepository.GetCustomerById(customerId);
        }

        // Add new customer
        public void AddCustomer(Customer_Master customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException("customer");
            }

            _customerRepository.AddCustomer(customer);
        }

        // Update customer
        public void UpdateCustomer(Customer_Master customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException("customer");
            }

            _customerRepository.UpdateCustomer(customer);
        }

        // Search customers
        public List<Customer_Master> SearchCustomers(string searchText)
        {
            return _customerRepository.SearchCustomers(searchText);
        }
    }
}