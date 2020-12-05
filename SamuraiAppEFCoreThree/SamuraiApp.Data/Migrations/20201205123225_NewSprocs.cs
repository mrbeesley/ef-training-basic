using Microsoft.EntityFrameworkCore.Migrations;

namespace SamuraiApp.Data.Migrations
{
    public partial class NewSprocs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"CREATE PROCEDURE dbo.SamuraiWhoSaidAWord
                    @text varchar(20)
                    AS 
                    SELECT Samurais.Id, Samurais.Name, Samurais.ClanId
                    FROM Samurais INNER JOIN
                        QUOTES on Samurais.Id = Quotes.SamuraiId
                    WHERE (Quotes.Text LIKE '%'+@text+'%')");
            migrationBuilder.Sql(
                @"CREATE PROCEDURE dbo.DeleteQuotesForSamurai
                    @samuraiId int
                    AS
                    DELETE FROM Quotes
                    WHERE Quotes.SamuraiId=@samuraiId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE dbo.SamuraiWhoSaidAWord");
            migrationBuilder.Sql("DROP PROCEDURE dbo.DeleteQuotesForSamurai");
        }
    }
}
