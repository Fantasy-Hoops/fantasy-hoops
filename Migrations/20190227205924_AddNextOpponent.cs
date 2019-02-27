using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class AddNextOpponent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NextOpponentID",
                table: "Teams",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_NextOpponentID",
                table: "Teams",
                column: "NextOpponentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Teams_NextOpponentID",
                table: "Teams",
                column: "NextOpponentID",
                principalTable: "Teams",
                principalColumn: "TeamID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Teams_NextOpponentID",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_NextOpponentID",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "NextOpponentID",
                table: "Teams");
        }
    }
}
