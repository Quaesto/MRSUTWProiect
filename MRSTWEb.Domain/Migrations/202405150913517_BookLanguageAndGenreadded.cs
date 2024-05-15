namespace MRSTWEb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BookLanguageAndGenreadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "Genre", c => c.String());
            AddColumn("dbo.Books", "Language", c => c.String());
        }
        
        public override void Down()
        {
        }
    }
}
