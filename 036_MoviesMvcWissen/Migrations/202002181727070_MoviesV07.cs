namespace _036_MoviesMvcWissen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoviesV07 : DbMigration
    {
        public override void Up()
        {
            string sql = "drop view if exists dbo.vwUsers";
            Sql(sql);
            sql = "create view dbo.vwUsers ";
            sql += "as ";
            sql += "select ISNULL(ROW_NUMBER() over(order by u.Id), 0) as Id, ";
            sql += " u.Id as UserId, UserName, [Password], Active, RoleId, r.[name] as RoleName ";
            sql += " from Users u inner join Roles r on u.RoleId =r.Id ";

            Sql(sql);
        }
        
        public override void Down()
        {
           // DropTable("dbo.vwUsers");
        }
    }
}
