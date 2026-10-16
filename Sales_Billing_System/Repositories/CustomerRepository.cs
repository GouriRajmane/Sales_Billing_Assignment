using MySql.Data.MySqlClient;
using Sales_Billing_System.Data;
using Sales_Billing_System.Models;
using Sales_Billing_System.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
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
                           .OrderBy(c => c.CustomerId)
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
                               (c.GSTIN != null &&  c.GSTIN.Contains(searchText)) ||
                               c.Address.Contains(searchText))
                           .OrderByDescending(c => c.CustomerId)
                           .ToList();
        }

        public PagedResult<Customer_Master> GetCustomersCursor(int pageSize, string searchText, int? lastId)
        {
            var result = new PagedResult<Customer_Master> { PageSize = pageSize };
            var connection = (MySqlConnection)_context.Database.Connection;
            bool shouldClose = connection.State != ConnectionState.Open;
            try
            {
                if (shouldClose) connection.Open();
                using (var command = new MySqlCommand("sp_GetCustomersCursor", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_PageSize", pageSize);
                    command.Parameters.AddWithValue("@p_SearchText", string.IsNullOrWhiteSpace(searchText) ? (object)DBNull.Value : searchText.Trim());
                    command.Parameters.AddWithValue("@p_LastId", lastId.HasValue ? (object)lastId.Value : DBNull.Value);

                    using (var reader = command.ExecuteReader())
                    {
                        Func<string, string> text = n => reader[n] == DBNull.Value ? null : Convert.ToString(reader[n]);

                        while (reader.Read())
                            result.Items.Add(new Customer_Master
                            {
                                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                                CustomerName = text("CustomerName"),
                                MobileNumber = text("MobileNumber"),
                                Address = text("Address"),
                                GSTIN = text("GSTIN")
                            });

                        if (reader.NextResult() && reader.Read())
                            result.TotalRecords = Convert.ToInt32(reader["TotalRecords"]);
                    }
                }
            }
            finally { if (shouldClose && connection.State == ConnectionState.Open) connection.Close(); }

            if (result.Items.Count > pageSize) { result.Items.RemoveAt(result.Items.Count - 1); result.HasMore = true; }
            return result;
        }
    }
}