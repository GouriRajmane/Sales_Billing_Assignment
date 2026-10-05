namespace Sales_Billing_System.MigrationsMySql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customer_Master",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        MobileNumber = c.String(nullable: false, maxLength: 15, storeType: "nvarchar"),
                        Address = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        GSTIN = c.String(maxLength: 15, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.CustomerId);
            
            CreateTable(
                "dbo.Sales_Invoice",
                c => new
                    {
                        InvoiceId = c.Int(nullable: false, identity: true),
                        InvoiceNumber = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        InvoiceDate = c.DateTime(nullable: false, precision: 0),
                        CustomerId = c.Int(nullable: false),
                        TotalTaxableAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalGSTAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GrandTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.String(nullable: false, maxLength: 20, storeType: "nvarchar"),
                        CreatedAt = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.InvoiceId)
                .ForeignKey("dbo.Customer_Master", t => t.CustomerId)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.Sales_Invoice_Item",
                c => new
                    {
                        InvoiceItemId = c.Int(nullable: false, identity: true),
                        InvoiceId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GSTPercentage = c.Decimal(nullable: false, precision: 5, scale: 2),
                        TaxableAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GSTAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.InvoiceItemId)
                .ForeignKey("dbo.Product_Master", t => t.ProductId)
                .ForeignKey("dbo.Sales_Invoice", t => t.InvoiceId, cascadeDelete: true)
                .Index(t => t.InvoiceId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Product_Master",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        ProductName = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        SKU = c.String(maxLength: 50, storeType: "nvarchar"),
                        Unit = c.String(nullable: false, maxLength: 20, storeType: "nvarchar"),
                        SellingPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GSTPercentage = c.Decimal(nullable: false, precision: 5, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false, precision: 0),
                        UpdatedAt = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sales_Invoice_Item", "InvoiceId", "dbo.Sales_Invoice");
            DropForeignKey("dbo.Sales_Invoice_Item", "ProductId", "dbo.Product_Master");
            DropForeignKey("dbo.Sales_Invoice", "CustomerId", "dbo.Customer_Master");
            DropIndex("dbo.Sales_Invoice_Item", new[] { "ProductId" });
            DropIndex("dbo.Sales_Invoice_Item", new[] { "InvoiceId" });
            DropIndex("dbo.Sales_Invoice", new[] { "CustomerId" });
            DropTable("dbo.Product_Master");
            DropTable("dbo.Sales_Invoice_Item");
            DropTable("dbo.Sales_Invoice");
            DropTable("dbo.Customer_Master");
        }
    }
}
