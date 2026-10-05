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
                               (c.GSTIN != null &&
                                c.GSTIN.Contains(searchText)))
                           .OrderByDescending(c => c.CustomerId)
                           .ToList();
        }

        public PagedResult<Customer_Master> GetCustomersPaged(
            int pageNumber,
            int pageSize,
            string searchText)
        {
            PagedResult<Customer_Master> result =
                new PagedResult<Customer_Master>();

            result.CurrentPage = pageNumber;
            result.PageSize = pageSize;

            var connection =
                (MySqlConnection)_context.Database.Connection;

            bool shouldCloseConnection =
                connection.State != ConnectionState.Open;

            try
            {
                if (shouldCloseConnection)
                {
                    connection.Open();
                }

                using (MySqlCommand command =
                    new MySqlCommand(
                        "sp_GetCustomersPaged",
                        connection))
                {
                    command.CommandType =
                        CommandType.StoredProcedure;

                    command.Parameters.AddWithValue(
                        "@p_PageNumber",
                        pageNumber);

                    command.Parameters.AddWithValue(
                        "@p_PageSize",
                        pageSize);

                    command.Parameters.AddWithValue(
                        "@p_SearchText",
                        string.IsNullOrWhiteSpace(searchText)
                            ? (object)DBNull.Value
                            : searchText.Trim());


                    using (MySqlDataReader reader =
                        command.ExecuteReader())
                    {
                        // ====================================================
                        // FIRST RESULT SET
                        // CUSTOMER DATA
                        // ====================================================

                        while (reader.Read())
                        {
                            Customer_Master customer =
                                new Customer_Master
                                {
                                    CustomerId =
                                        Convert.ToInt32(
                                            reader["CustomerId"]),

                                    CustomerName =
                                        Convert.ToString(
                                            reader["CustomerName"]),

                                    MobileNumber =
                                        reader["MobileNumber"] ==
                                            DBNull.Value
                                            ? null
                                            : Convert.ToString(
                                                reader["MobileNumber"]),

                                    Address =
                                        reader["Address"] ==
                                            DBNull.Value
                                            ? null
                                            : Convert.ToString(
                                                reader["Address"]),

                                    GSTIN =
                                        reader["GSTIN"] ==
                                            DBNull.Value
                                            ? null
                                            : Convert.ToString(
                                                reader["GSTIN"])
                                };

                            result.Items.Add(customer);
                        }


                        // ====================================================
                        // SECOND RESULT SET
                        // TOTAL RECORDS
                        // ====================================================

                        if (reader.NextResult() &&
                            reader.Read())
                        {
                            result.TotalRecords =
                                Convert.ToInt32(
                                    reader["TotalRecords"]);
                        }
                    }
                }
            }
            finally
            {
                if (shouldCloseConnection &&
                    connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }
    }
}