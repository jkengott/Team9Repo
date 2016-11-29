namespace Team9.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Team910 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Purchases", "PurchaseCard_CreditCardID", c => c.Int());
            CreateIndex("dbo.Purchases", "PurchaseCard_CreditCardID");
            AddForeignKey("dbo.Purchases", "PurchaseCard_CreditCardID", "dbo.CreditCards", "CreditCardID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Purchases", "PurchaseCard_CreditCardID", "dbo.CreditCards");
            DropIndex("dbo.Purchases", new[] { "PurchaseCard_CreditCardID" });
            DropColumn("dbo.Purchases", "PurchaseCard_CreditCardID");
        }
    }
}
