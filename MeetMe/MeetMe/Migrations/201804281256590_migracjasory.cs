namespace MeetMe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migracjasory : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Events", "Rating");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "Rating", c => c.Single(nullable: false));
        }
    }
}
