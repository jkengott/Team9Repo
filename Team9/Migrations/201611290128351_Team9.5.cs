namespace Team9.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Team95 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CreditCards",
                c => new
                    {
                        CreditCardID = c.Int(nullable: false, identity: true),
                        CCNumber = c.String(),
                        CardType = c.Int(nullable: false),
                        hideNumber = c.String(),
                        CardOwner_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CreditCardID)
                .ForeignKey("dbo.AspNetUsers", t => t.CardOwner_Id)
                .Index(t => t.CardOwner_Id);
            
            AddColumn("dbo.AspNetUsers", "CC1_CreditCardID", c => c.Int());
            AddColumn("dbo.AspNetUsers", "CC2_CreditCardID", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "CC1_CreditCardID");
            CreateIndex("dbo.AspNetUsers", "CC2_CreditCardID");
            AddForeignKey("dbo.AspNetUsers", "CC1_CreditCardID", "dbo.CreditCards", "CreditCardID");
            AddForeignKey("dbo.AspNetUsers", "CC2_CreditCardID", "dbo.CreditCards", "CreditCardID");
            DropColumn("dbo.AspNetUsers", "CreditCard1");
            DropColumn("dbo.AspNetUsers", "CCType1");
            DropColumn("dbo.AspNetUsers", "CreditCard2");
            DropColumn("dbo.AspNetUsers", "CCType2");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "CCType2", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "CreditCard2", c => c.String());
            AddColumn("dbo.AspNetUsers", "CCType1", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "CreditCard1", c => c.String());
            DropForeignKey("dbo.AspNetUsers", "CC2_CreditCardID", "dbo.CreditCards");
            DropForeignKey("dbo.AspNetUsers", "CC1_CreditCardID", "dbo.CreditCards");
            DropForeignKey("dbo.CreditCards", "CardOwner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.CreditCards", new[] { "CardOwner_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "CC2_CreditCardID" });
            DropIndex("dbo.AspNetUsers", new[] { "CC1_CreditCardID" });
            DropColumn("dbo.AspNetUsers", "CC2_CreditCardID");
            DropColumn("dbo.AspNetUsers", "CC1_CreditCardID");
            DropTable("dbo.CreditCards");
        }
    }
}
