namespace Team9.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Team97 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CreditCards", "displayNumber", c => c.String());
            DropColumn("dbo.CreditCards", "hideNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CreditCards", "hideNumber", c => c.String());
            DropColumn("dbo.CreditCards", "displayNumber");
        }
    }
}
