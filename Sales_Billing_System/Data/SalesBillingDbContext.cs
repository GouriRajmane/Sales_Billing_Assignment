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


            // Decimal Precision
            modelBuilder.Entity<Product_Master>()
                .Property(p => p.SellingPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Sales_Invoice>()
                .Property(i => i.TotalTaxableAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Sales_Invoice>()
                .Property(i => i.TotalGSTAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Sales_Invoice>()
                .Property(i => i.GrandTotal)
                .HasPrecision(18, 2);


            modelBuilder.Entity<Sales_Invoice_Item>()
                .Property(i => i.Quantity)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Sales_Invoice_Item>()
                .Property(i => i.Rate)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Sales_Invoice_Item>()
                .Property(i => i.Discount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Sales_Invoice_Item>()
                .Property(i => i.GSTPercentage)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Sales_Invoice_Item>()
                .Property(i => i.TaxableAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Sales_Invoice_Item>()
                .Property(i => i.GSTAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Sales_Invoice_Item>()
                .Property(i => i.TotalAmount)
                .HasPrecision(18, 2);


            base.OnModelCreating(modelBuilder);
        }
    }
}