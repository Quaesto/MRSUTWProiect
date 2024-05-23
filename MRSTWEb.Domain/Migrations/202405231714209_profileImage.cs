﻿namespace MRSTWEb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class profileImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientProfiles", "ProfileImage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientProfiles", "ProfileImage");
        }
    }
}
