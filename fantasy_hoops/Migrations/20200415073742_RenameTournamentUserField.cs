using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class RenameTournamentUserField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ties",
                table: "TournamentUsers");

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "TournamentUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DroppedUserId",
                table: "Contests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contests_DroppedUserId",
                table: "Contests",
                column: "DroppedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contests_AspNetUsers_DroppedUserId",
                table: "Contests",
                column: "DroppedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contests_AspNetUsers_DroppedUserId",
                table: "Contests");

            migrationBuilder.DropIndex(
                name: "IX_Contests_DroppedUserId",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "TournamentUsers");

            migrationBuilder.DropColumn(
                name: "DroppedUserId",
                table: "Contests");

            migrationBuilder.AddColumn<int>(
                name: "Ties",
                table: "TournamentUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
