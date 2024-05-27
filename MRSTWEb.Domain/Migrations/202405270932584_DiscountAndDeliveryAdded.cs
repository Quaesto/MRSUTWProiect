namespace MRSTWEb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DiscountAndDeliveryAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Discounts",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Percentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SetTime = c.DateTime(nullable: false),
                        ExpirationTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.DeliveryCosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ClientProfiles", "ProfileImage", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Discounts", "Id", "dbo.Books");
            DropIndex("dbo.Discounts", new[] { "Id" });
            DropColumn("dbo.ClientProfiles", "ProfileImage");
            DropTable("dbo.DeliveryCosts");
            DropTable("dbo.Discounts");
        }
    }
}
