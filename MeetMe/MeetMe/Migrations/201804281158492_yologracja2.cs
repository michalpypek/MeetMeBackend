namespace MeetMe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class yologracja2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Ratings", "Sum", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Ratings", "Sum", c => c.Int(nullable: false));
        }
    }
}
