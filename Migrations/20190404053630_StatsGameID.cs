using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class StatsGameID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameID",
                table: "Stats",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Stats_GameID",
                table: "Stats",
                column: "GameID");

            migrationBuilder.AddForeignKey(
                name: "FK_Stats_Games_GameID",
                table: "Stats",
                column: "GameID",
                principalTable: "Games",
                principalColumn: "GameID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stats_Games_GameID",
                table: "Stats");

            migrationBuilder.DropIndex(
                name: "IX_Stats_GameID",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "GameID",
                table: "Stats");
        }
    }
}
