namespace DataConcentrator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AlarmHistorySamples",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        AlarmId = c.String(nullable: false),
                        VarName = c.String(nullable: false),
                        Message = c.String(),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Alarms",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        ActivationValue = c.Double(nullable: false),
                        ActivationEdge = c.Int(nullable: false),
                        Priority = c.Int(nullable: false),
                        AnalogInputId = c.String(nullable: false, maxLength: 128),
                        AnalogInputName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnalogInputs", t => t.AnalogInputId, cascadeDelete: true)
                .Index(t => t.AnalogInputId);
            
            CreateTable(
                "dbo.AnalogInputs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        LowLimit = c.Double(nullable: false),
                        HighLimit = c.Double(nullable: false),
                        Units = c.String(nullable: false),
                        ScanTime = c.Double(nullable: false),
                        OnOffScan = c.Boolean(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Address = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AnalogOutputs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        LowLimit = c.Double(nullable: false),
                        HighLimit = c.Double(nullable: false),
                        Units = c.String(nullable: false),
                        InitialValue = c.Double(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Address = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DigitalInputs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ScanTime = c.Double(nullable: false),
                        OnOffScan = c.Boolean(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Address = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DigitalOutputs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        InitialValue = c.Double(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Address = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Alarms", "AnalogInputId", "dbo.AnalogInputs");
            DropIndex("dbo.Alarms", new[] { "AnalogInputId" });
            DropTable("dbo.DigitalOutputs");
            DropTable("dbo.DigitalInputs");
            DropTable("dbo.AnalogOutputs");
            DropTable("dbo.AnalogInputs");
            DropTable("dbo.Alarms");
            DropTable("dbo.AlarmHistorySamples");
        }
    }
}
