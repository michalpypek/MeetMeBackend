namespace MeetMe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rototo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Guests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Event_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.Event_Id)
                .Index(t => t.Event_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Guests", "Event_Id", "dbo.Events");
            DropIndex("dbo.Guests", new[] { "Event_Id" });
            DropTable("dbo.Guests");
        }
    }
}
