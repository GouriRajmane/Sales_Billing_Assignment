using System.Data.Entity;
using Sales_Billing_System.Models;

namespace Sales_Billing_System.Data
{
    public class SalesBillingDbContext : DbContext
    {
        public SalesBillingDbContext()
            : base("name=SalesBillingDbContext")
        {
        }

        public DbSet<Product_Master> Products { get; set; }

        public DbSet<Customer_Master> Customers { get; set; }

        public DbSet<Sales_Invoice> SalesInvoices { get; set; }

        public DbSet<Sales_Invoice_Item> SalesInvoiceItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Customer -> Sales Invoices
            modelBuilder.Entity<Sales_Invoice>()
                .HasRequired(i => i.Customer)
                .WithMany(c => c.SalesInvoices)
                .HasForeignKey(i => i.CustomerId)
                .WillCascadeOnDelete(false);

            // Sales Invoice -> Invoice Items
            modelBuilder.Entity<Sales_Invoice_Item>()
                .HasRequired(i => i.SalesInvoice)
                .WithMany(s => s.InvoiceItems)
                .HasForeignKey(i => i.InvoiceId)
                .WillCascadeOnDelete(true);

            // Product -> Invoice Items
            modelBuilder.Entity<Sales_Invoice_Item>()
                .HasRequired(i => i.Product)
                .WithMany(p => p.SalesInvoiceItems)
                .HasForeignKey(i => i.ProductId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}