namespace Team9.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Team913 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PurchaseItemArtists", "PurchaseItem_PurchaseItemID", "dbo.PurchaseItems");
            DropForeignKey("dbo.PurchaseItemArtists", "Artist_ArtistID", "dbo.Artists");
            DropIndex("dbo.PurchaseItemArtists", new[] { "PurchaseItem_PurchaseItemID" });
            DropIndex("dbo.PurchaseItemArtists", new[] { "Artist_ArtistID" });
            DropTable("dbo.PurchaseItemArtists");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PurchaseItemArtists",
                c => new
                    {
                        PurchaseItem_PurchaseItemID = c.Int(nullable: false),
                        Artist_ArtistID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PurchaseItem_PurchaseItemID, t.Artist_ArtistID });
            
            CreateIndex("dbo.PurchaseItemArtists", "Artist_ArtistID");
            CreateIndex("dbo.PurchaseItemArtists", "PurchaseItem_PurchaseItemID");
            AddForeignKey("dbo.PurchaseItemArtists", "Artist_ArtistID", "dbo.Artists", "ArtistID", cascadeDelete: true);
            AddForeignKey("dbo.PurchaseItemArtists", "PurchaseItem_PurchaseItemID", "dbo.PurchaseItems", "PurchaseItemID", cascadeDelete: true);
        }
    }
}
