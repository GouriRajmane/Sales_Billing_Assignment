using Sales_Billing_System.Data;
using Sales_Billing_System.Models;
using Sales_Billing_System.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Sales_Billing_System.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly SalesBillingDbContext _context;

        public CustomerRepository()
        {
            _context = new SalesBillingDbContext();
        }

        // Get all customers
        public List<Customer_Master> GetAllCustomers()
        {
            return _context.Customers
                           .OrderByDescending(c => c.CustomerId)
                           .ToList();
        }

        // Get customer by ID
        public Customer_Master GetCustomerById(int customerId)
        {
            return _context.Customers
                           .FirstOrDefault(c => c.CustomerId == customerId);
        }

        // Add customer
        public void AddCustomer(Customer_Master customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }

        // Update customer
        public void UpdateCustomer(Customer_Master customer)
        {
            Customer_Master existingCustomer =
                GetCustomerById(customer.CustomerId);

            if (existingCustomer != null)
            {
                existingCustomer.CustomerName = customer.CustomerName;
                existingCustomer.MobileNumber = customer.MobileNumber;
                existingCustomer.Address = customer.Address;
                existingCustomer.GSTIN = customer.GSTIN;

                _context.SaveChanges();
            }
        }

        // Search customers
        public List<Customer_Master> SearchCustomers(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return GetAllCustomers();
            }

            return _context.Customers
                           .Where(c =>
                               c.CustomerName.Contains(searchText) ||
                               c.MobileNumber.Contains(searchText) ||
                               (c.GSTIN != null &&
                                c.GSTIN.Contains(searchText)))
                           .OrderByDescending(c => c.CustomerId)
                           .ToList();
        }
    }
}