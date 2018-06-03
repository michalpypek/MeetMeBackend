namespace MeetMe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class facebook : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "FacebookURL", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "FacebookURL");
        }
    }
}
