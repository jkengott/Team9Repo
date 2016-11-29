namespace Team9.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Team99 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.CreditCards", "CardType");
            DropColumn("dbo.CreditCards", "displayNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CreditCards", "displayNumber", c => c.String());
            AddColumn("dbo.CreditCards", "CardType", c => c.Int(nullable: false));
        }
    }
}
