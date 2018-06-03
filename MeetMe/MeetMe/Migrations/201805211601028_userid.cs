namespace MeetMe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userid : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Guests", "User_Id", "dbo.Users");
            DropIndex("dbo.Guests", new[] { "User_Id" });
            RenameColumn(table: "dbo.Guests", name: "User_Id", newName: "UserId");
            AlterColumn("dbo.Guests", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Guests", "UserId");
            AddForeignKey("dbo.Guests", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Guests", "UserId", "dbo.Users");
            DropIndex("dbo.Guests", new[] { "UserId" });
            AlterColumn("dbo.Guests", "UserId", c => c.Int());
            RenameColumn(table: "dbo.Guests", name: "UserId", newName: "User_Id");
            CreateIndex("dbo.Guests", "User_Id");
            AddForeignKey("dbo.Guests", "User_Id", "dbo.Users", "Id");
        }
    }
}
