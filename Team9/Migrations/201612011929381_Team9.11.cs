namespace Team9.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Team911 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PurchaseItemArtists",
                c => new
                    {
                        PurchaseItem_PurchaseItemID = c.Int(nullable: false),
                        Artist_ArtistID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PurchaseItem_PurchaseItemID, t.Artist_ArtistID })
                .ForeignKey("dbo.PurchaseItems", t => t.PurchaseItem_PurchaseItemID, cascadeDelete: true)
                .ForeignKey("dbo.Artists", t => t.Artist_ArtistID, cascadeDelete: true)
                .Index(t => t.PurchaseItem_PurchaseItemID)
                .Index(t => t.Artist_ArtistID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PurchaseItemArtists", "Artist_ArtistID", "dbo.Artists");
            DropForeignKey("dbo.PurchaseItemArtists", "PurchaseItem_PurchaseItemID", "dbo.PurchaseItems");
            DropIndex("dbo.PurchaseItemArtists", new[] { "Artist_ArtistID" });
            DropIndex("dbo.PurchaseItemArtists", new[] { "PurchaseItem_PurchaseItemID" });
            DropTable("dbo.PurchaseItemArtists");
        }
    }
}
