namespace MeetMe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rototo1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Guests", "Event_Id", "dbo.Events");
            DropIndex("dbo.Guests", new[] { "Event_Id" });
            AddColumn("dbo.Users", "Event_Id", c => c.Int());
            CreateIndex("dbo.Users", "Event_Id");
            AddForeignKey("dbo.Users", "Event_Id", "dbo.Events", "Id");
            DropTable("dbo.Guests");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Guests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Event_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.Users", "Event_Id", "dbo.Events");
            DropIndex("dbo.Users", new[] { "Event_Id" });
            DropColumn("dbo.Users", "Event_Id");
            CreateIndex("dbo.Guests", "Event_Id");
            AddForeignKey("dbo.Guests", "Event_Id", "dbo.Events", "Id");
        }
    }
}
