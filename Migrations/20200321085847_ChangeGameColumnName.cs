using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class ChangeGameColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGameFinished",
                table: "Games");

            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "Games",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "Games");

            migrationBuilder.AddColumn<bool>(
                name: "IsGameFinished",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
