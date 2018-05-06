namespace MeetMe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class yologracja : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "refreshToken");
            DropColumn("dbo.Users", "tokenExpirationDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "tokenExpirationDate", c => c.Long(nullable: false));
            AddColumn("dbo.Users", "refreshToken", c => c.String());
        }
    }
}
