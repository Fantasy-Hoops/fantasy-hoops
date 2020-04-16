using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class AddTournamentStatusField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "Tournaments");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Tournaments",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tournaments");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Tournaments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "Tournaments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
