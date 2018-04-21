namespace MeetMe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pleasework : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "Rating_Id", "dbo.Ratings");
            DropForeignKey("dbo.Events", "Location_Id", "dbo.Locations");
            DropIndex("dbo.Users", new[] { "Rating_Id" });
            DropIndex("dbo.Events", new[] { "Location_Id" });
            AddColumn("dbo.Users", "token", c => c.String());
            AddColumn("dbo.Users", "refreshToken", c => c.String());
            AddColumn("dbo.Users", "tokenExpirationDate", c => c.Long(nullable: false));
            AddColumn("dbo.Users", "FirstName", c => c.String());
            AddColumn("dbo.Users", "LastName", c => c.String());
            AddColumn("dbo.Users", "RatingID", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "UserRating", c => c.Single(nullable: false));
            AddColumn("dbo.Events", "Latitude", c => c.Single(nullable: false));
            AddColumn("dbo.Events", "Longitude", c => c.Single(nullable: false));
            AddColumn("dbo.Events", "LocationName", c => c.String());
            AddColumn("dbo.Events", "Description", c => c.String());
            AddColumn("dbo.Events", "GoogleMapsURL", c => c.String());
            AddColumn("dbo.AspNetUsers", "UserId", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "token", c => c.String());
            AddColumn("dbo.AspNetUsers", "refreshToken", c => c.String());
            AddColumn("dbo.AspNetUsers", "tokenExpirationDate", c => c.Long(nullable: false));
            DropColumn("dbo.Users", "UserName");
            DropColumn("dbo.Users", "Rating_Id");
            DropColumn("dbo.Events", "Location_Id");
            DropTable("dbo.Locations");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Latitude = c.Single(nullable: false),
                        Longitude = c.Single(nullable: false),
                        LocationName = c.String(),
                        Description = c.String(),
                        GoogleMapsURL = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Events", "Location_Id", c => c.Int());
            AddColumn("dbo.Users", "Rating_Id", c => c.Int());
            AddColumn("dbo.Users", "UserName", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.AspNetUsers", "tokenExpirationDate");
            DropColumn("dbo.AspNetUsers", "refreshToken");
            DropColumn("dbo.AspNetUsers", "token");
            DropColumn("dbo.AspNetUsers", "UserId");
            DropColumn("dbo.Events", "GoogleMapsURL");
            DropColumn("dbo.Events", "Description");
            DropColumn("dbo.Events", "LocationName");
            DropColumn("dbo.Events", "Longitude");
            DropColumn("dbo.Events", "Latitude");
            DropColumn("dbo.Users", "UserRating");
            DropColumn("dbo.Users", "RatingID");
            DropColumn("dbo.Users", "LastName");
            DropColumn("dbo.Users", "FirstName");
            DropColumn("dbo.Users", "tokenExpirationDate");
            DropColumn("dbo.Users", "refreshToken");
            DropColumn("dbo.Users", "token");
            CreateIndex("dbo.Events", "Location_Id");
            CreateIndex("dbo.Users", "Rating_Id");
            AddForeignKey("dbo.Events", "Location_Id", "dbo.Locations", "Id");
            AddForeignKey("dbo.Users", "Rating_Id", "dbo.Ratings", "Id");
        }
    }
}
