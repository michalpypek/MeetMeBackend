namespace MeetMe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rototo2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "Event_Id", "dbo.Events");
            DropIndex("dbo.Users", new[] { "Event_Id" });
            CreateTable(
                "dbo.Guests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User_Id = c.Int(),
                        Event_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .ForeignKey("dbo.Events", t => t.Event_Id)
                .Index(t => t.User_Id)
                .Index(t => t.Event_Id);
            
            DropColumn("dbo.Users", "Event_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Event_Id", c => c.Int());
            DropForeignKey("dbo.Guests", "Event_Id", "dbo.Events");
            DropForeignKey("dbo.Guests", "User_Id", "dbo.Users");
            DropIndex("dbo.Guests", new[] { "Event_Id" });
            DropIndex("dbo.Guests", new[] { "User_Id" });
            DropTable("dbo.Guests");
            CreateIndex("dbo.Users", "Event_Id");
            AddForeignKey("dbo.Users", "Event_Id", "dbo.Events", "Id");
        }
    }
}
