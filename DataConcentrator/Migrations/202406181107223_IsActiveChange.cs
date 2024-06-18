namespace DataConcentrator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsActiveChange : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Alarms", "IsActive");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Alarms", "IsActive", c => c.Boolean(nullable: false));
        }
    }
}
