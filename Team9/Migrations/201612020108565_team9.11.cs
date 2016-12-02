namespace Team9.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class team911 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Albums", "isFeatured", c => c.Boolean(nullable: false));
            AddColumn("dbo.Artists", "isFeatured", c => c.Boolean(nullable: false));
            AddColumn("dbo.Songs", "isFeatured", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Songs", "isFeatured");
            DropColumn("dbo.Artists", "isFeatured");
            DropColumn("dbo.Albums", "isFeatured");
        }
    }
}
