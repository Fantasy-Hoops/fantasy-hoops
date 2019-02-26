using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class UpdatePlayerModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AbbrName",
                table: "Players",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Players",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbbrName",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Players");
        }
    }
}
