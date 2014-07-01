namespace FantasyBaseball.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_FavoriteTeams : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FavoriteTeams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FavoriteTeams", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.FavoriteTeams", new[] { "ApplicationUser_Id" });
            DropTable("dbo.FavoriteTeams");
        }
    }
}
