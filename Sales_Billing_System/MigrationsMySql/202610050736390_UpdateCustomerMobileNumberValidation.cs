namespace Sales_Billing_System.MigrationsMySql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCustomerMobileNumberValidation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Customer_Master", "MobileNumber", c => c.String(nullable: false, maxLength: 10, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Customer_Master", "MobileNumber", c => c.String(nullable: false, maxLength: 15, storeType: "nvarchar"));
        }
    }
}
