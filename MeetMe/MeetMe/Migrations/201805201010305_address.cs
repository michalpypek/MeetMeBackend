namespace MeetMe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class address : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "Address", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "Address");
        }
    }
}
