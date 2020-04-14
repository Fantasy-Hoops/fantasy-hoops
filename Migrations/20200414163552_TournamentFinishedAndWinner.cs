using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class TournamentFinishedAndWinner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "Tournaments",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "WinnerID",
                table: "Tournaments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_WinnerID",
                table: "Tournaments",
                column: "WinnerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_AspNetUsers_WinnerID",
                table: "Tournaments",
                column: "WinnerID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_AspNetUsers_WinnerID",
                table: "Tournaments");

            migrationBuilder.DropIndex(
                name: "IX_Tournaments_WinnerID",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "WinnerID",
                table: "Tournaments");
        }
    }
}
