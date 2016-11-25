namespace Team9.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Team92 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Artists", "Song_SongID", "dbo.Songs");
            DropForeignKey("dbo.Artists", "Album_AlbumID", "dbo.Albums");
            DropIndex("dbo.Artists", new[] { "Song_SongID" });
            DropIndex("dbo.Artists", new[] { "Album_AlbumID" });
            CreateTable(
                "dbo.ArtistAlbums",
                c => new
                    {
                        Artist_ArtistID = c.Int(nullable: false),
                        Album_AlbumID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Artist_ArtistID, t.Album_AlbumID })
                .ForeignKey("dbo.Artists", t => t.Artist_ArtistID, cascadeDelete: true)
                .ForeignKey("dbo.Albums", t => t.Album_AlbumID, cascadeDelete: true)
                .Index(t => t.Artist_ArtistID)
                .Index(t => t.Album_AlbumID);
            
            CreateTable(
                "dbo.SongArtists",
                c => new
                    {
                        Song_SongID = c.Int(nullable: false),
                        Artist_ArtistID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Song_SongID, t.Artist_ArtistID })
                .ForeignKey("dbo.Songs", t => t.Song_SongID, cascadeDelete: true)
                .ForeignKey("dbo.Artists", t => t.Artist_ArtistID, cascadeDelete: true)
                .Index(t => t.Song_SongID)
                .Index(t => t.Artist_ArtistID);
            
            DropColumn("dbo.Artists", "Song_SongID");
            DropColumn("dbo.Artists", "Album_AlbumID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Artists", "Album_AlbumID", c => c.Int());
            AddColumn("dbo.Artists", "Song_SongID", c => c.Int());
            DropForeignKey("dbo.SongArtists", "Artist_ArtistID", "dbo.Artists");
            DropForeignKey("dbo.SongArtists", "Song_SongID", "dbo.Songs");
            DropForeignKey("dbo.ArtistAlbums", "Album_AlbumID", "dbo.Albums");
            DropForeignKey("dbo.ArtistAlbums", "Artist_ArtistID", "dbo.Artists");
            DropIndex("dbo.SongArtists", new[] { "Artist_ArtistID" });
            DropIndex("dbo.SongArtists", new[] { "Song_SongID" });
            DropIndex("dbo.ArtistAlbums", new[] { "Album_AlbumID" });
            DropIndex("dbo.ArtistAlbums", new[] { "Artist_ArtistID" });
            DropTable("dbo.SongArtists");
            DropTable("dbo.ArtistAlbums");
            CreateIndex("dbo.Artists", "Album_AlbumID");
            CreateIndex("dbo.Artists", "Song_SongID");
            AddForeignKey("dbo.Artists", "Album_AlbumID", "dbo.Albums", "AlbumID");
            AddForeignKey("dbo.Artists", "Song_SongID", "dbo.Songs", "SongID");
        }
    }
}
