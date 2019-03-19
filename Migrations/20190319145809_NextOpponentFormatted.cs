using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class NextOpponentFormatted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NextOppFormatted",
                table: "Teams",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextOppFormatted",
                table: "Teams");
        }
    }
}
