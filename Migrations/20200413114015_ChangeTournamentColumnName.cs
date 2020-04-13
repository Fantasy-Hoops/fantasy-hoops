using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class ChangeTournamentColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Tournaments");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Tournaments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Tournaments");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Tournaments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
