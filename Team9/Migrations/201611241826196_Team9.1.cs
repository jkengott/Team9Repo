namespace Team9.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Team91 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Albums",
                c => new
                    {
                        AlbumID = c.Int(nullable: false, identity: true),
                        AlbumName = c.String(nullable: false),
                        isDiscounted = c.Boolean(nullable: false),
                        AlbumPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountAlbumPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.AlbumID);
            
            CreateTable(
                "dbo.Artists",
                c => new
                    {
                        ArtistID = c.Int(nullable: false, identity: true),
                        ArtistName = c.String(nullable: false),
                        Song_SongID = c.Int(),
                        Album_AlbumID = c.Int(),
                    })
                .PrimaryKey(t => t.ArtistID)
                .ForeignKey("dbo.Songs", t => t.Song_SongID)
                .ForeignKey("dbo.Albums", t => t.Album_AlbumID)
                .Index(t => t.Song_SongID)
                .Index(t => t.Album_AlbumID);
            
            CreateTable(
                "dbo.Genres",
                c => new
                    {
                        GenreID = c.Int(nullable: false, identity: true),
                        GenreName = c.String(),
                        Artist_ArtistID = c.Int(),
                        Song_SongID = c.Int(),
                        Album_AlbumID = c.Int(),
                    })
                .PrimaryKey(t => t.GenreID)
                .ForeignKey("dbo.Artists", t => t.Artist_ArtistID)
                .ForeignKey("dbo.Songs", t => t.Song_SongID)
                .ForeignKey("dbo.Albums", t => t.Album_AlbumID)
                .Index(t => t.Artist_ArtistID)
                .Index(t => t.Song_SongID)
                .Index(t => t.Album_AlbumID);
            
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        RatingID = c.Int(nullable: false, identity: true),
                        RatingText = c.String(),
                        RatingValue = c.Int(nullable: false),
                        RatingAlbum_AlbumID = c.Int(),
                        RatingArtist_ArtistID = c.Int(),
                        RatingSong_SongID = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.RatingID)
                .ForeignKey("dbo.Albums", t => t.RatingAlbum_AlbumID)
                .ForeignKey("dbo.Artists", t => t.RatingArtist_ArtistID)
                .ForeignKey("dbo.Songs", t => t.RatingSong_SongID)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.RatingAlbum_AlbumID)
                .Index(t => t.RatingArtist_ArtistID)
                .Index(t => t.RatingSong_SongID)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Songs",
                c => new
                    {
                        SongID = c.Int(nullable: false, identity: true),
                        SongName = c.String(nullable: false),
                        SongPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        isDiscoutned = c.Boolean(nullable: false),
                        DiscountPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SongLength = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SongAlbum_AlbumID = c.Int(),
                    })
                .PrimaryKey(t => t.SongID)
                .ForeignKey("dbo.Albums", t => t.SongAlbum_AlbumID)
                .Index(t => t.SongAlbum_AlbumID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FName = c.String(),
                        LName = c.String(),
                        CreditCard1 = c.String(),
                        CCType1 = c.Int(nullable: false),
                        CreditCard2 = c.String(),
                        CCType2 = c.Int(nullable: false),
                        SSN = c.String(),
                        City = c.String(),
                        StreeAddress = c.String(),
                        Zip = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Purchases",
                c => new
                    {
                        PurchaseID = c.Int(nullable: false, identity: true),
                        isPurchased = c.Boolean(nullable: false),
                        PurchaseDate = c.DateTime(),
                        isGift = c.Boolean(nullable: false),
                        GiftUser_Id = c.String(maxLength: 128),
                        PurchaseUser_Id = c.String(maxLength: 128),
                        AppUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.PurchaseID)
                .ForeignKey("dbo.AspNetUsers", t => t.GiftUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.PurchaseUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AppUser_Id)
                .Index(t => t.GiftUser_Id)
                .Index(t => t.PurchaseUser_Id)
                .Index(t => t.AppUser_Id);
            
            CreateTable(
                "dbo.PurchaseItems",
                c => new
                    {
                        PurchaseItemID = c.Int(nullable: false, identity: true),
                        PurchaseItemPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        isAlbum = c.Boolean(nullable: false),
                        Purchase_PurchaseID = c.Int(),
                        PurchaseItemAlbum_AlbumID = c.Int(),
                        PurchaseItemSong_SongID = c.Int(),
                    })
                .PrimaryKey(t => t.PurchaseItemID)
                .ForeignKey("dbo.Purchases", t => t.Purchase_PurchaseID)
                .ForeignKey("dbo.Albums", t => t.PurchaseItemAlbum_AlbumID)
                .ForeignKey("dbo.Songs", t => t.PurchaseItemSong_SongID)
                .Index(t => t.Purchase_PurchaseID)
                .Index(t => t.PurchaseItemAlbum_AlbumID)
                .Index(t => t.PurchaseItemSong_SongID);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Genres", "Album_AlbumID", "dbo.Albums");
            DropForeignKey("dbo.Artists", "Album_AlbumID", "dbo.Albums");
            DropForeignKey("dbo.Ratings", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Purchases", "AppUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Purchases", "PurchaseUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.PurchaseItems", "PurchaseItemSong_SongID", "dbo.Songs");
            DropForeignKey("dbo.PurchaseItems", "PurchaseItemAlbum_AlbumID", "dbo.Albums");
            DropForeignKey("dbo.PurchaseItems", "Purchase_PurchaseID", "dbo.Purchases");
            DropForeignKey("dbo.Purchases", "GiftUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Ratings", "RatingSong_SongID", "dbo.Songs");
            DropForeignKey("dbo.Genres", "Song_SongID", "dbo.Songs");
            DropForeignKey("dbo.Artists", "Song_SongID", "dbo.Songs");
            DropForeignKey("dbo.Songs", "SongAlbum_AlbumID", "dbo.Albums");
            DropForeignKey("dbo.Ratings", "RatingArtist_ArtistID", "dbo.Artists");
            DropForeignKey("dbo.Ratings", "RatingAlbum_AlbumID", "dbo.Albums");
            DropForeignKey("dbo.Genres", "Artist_ArtistID", "dbo.Artists");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.PurchaseItems", new[] { "PurchaseItemSong_SongID" });
            DropIndex("dbo.PurchaseItems", new[] { "PurchaseItemAlbum_AlbumID" });
            DropIndex("dbo.PurchaseItems", new[] { "Purchase_PurchaseID" });
            DropIndex("dbo.Purchases", new[] { "AppUser_Id" });
            DropIndex("dbo.Purchases", new[] { "PurchaseUser_Id" });
            DropIndex("dbo.Purchases", new[] { "GiftUser_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Songs", new[] { "SongAlbum_AlbumID" });
            DropIndex("dbo.Ratings", new[] { "User_Id" });
            DropIndex("dbo.Ratings", new[] { "RatingSong_SongID" });
            DropIndex("dbo.Ratings", new[] { "RatingArtist_ArtistID" });
            DropIndex("dbo.Ratings", new[] { "RatingAlbum_AlbumID" });
            DropIndex("dbo.Genres", new[] { "Album_AlbumID" });
            DropIndex("dbo.Genres", new[] { "Song_SongID" });
            DropIndex("dbo.Genres", new[] { "Artist_ArtistID" });
            DropIndex("dbo.Artists", new[] { "Album_AlbumID" });
            DropIndex("dbo.Artists", new[] { "Song_SongID" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.PurchaseItems");
            DropTable("dbo.Purchases");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Songs");
            DropTable("dbo.Ratings");
            DropTable("dbo.Genres");
            DropTable("dbo.Artists");
            DropTable("dbo.Albums");
        }
    }
}
