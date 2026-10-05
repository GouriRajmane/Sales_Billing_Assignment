using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sales_Billing_System.Models
{
    public class Customer_Master
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^[6-9]\d{9}$",
           ErrorMessage = "Please enter a valid 10-digit Indian mobile number.")]
        public string MobileNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(15)]
        public string GSTIN { get; set; }

        // One Customer -> Many Sales Invoices
        public virtual ICollection<Sales_Invoice> SalesInvoices { get; set; }

        public Customer_Master()
        {
            SalesInvoices = new List<Sales_Invoice>();
        }
    }
}