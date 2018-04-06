namespace MeetMe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tatata : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "QrCode_code", c => c.String());
            DropColumn("dbo.Events", "MyProperty");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "MyProperty", c => c.Boolean(nullable: false));
            DropColumn("dbo.Events", "QrCode_code");
        }
    }
}
