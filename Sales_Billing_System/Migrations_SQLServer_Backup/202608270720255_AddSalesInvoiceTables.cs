namespace Sales_Billing_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSalesInvoiceTables : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Sales_Invoice_Item", "GSTPercentage", c => c.Decimal(nullable: false, precision: 5, scale: 2));
            AlterColumn("dbo.Product_Master", "GSTPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Product_Master", "GSTPercentage", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Sales_Invoice_Item", "GSTPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
