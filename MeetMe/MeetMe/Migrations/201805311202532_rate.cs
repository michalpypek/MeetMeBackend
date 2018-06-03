namespace MeetMe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rate : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Users", "RatingID");
            AddForeignKey("dbo.Users", "RatingID", "dbo.Ratings", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "RatingID", "dbo.Ratings");
            DropIndex("dbo.Users", new[] { "RatingID" });
        }
    }
}
