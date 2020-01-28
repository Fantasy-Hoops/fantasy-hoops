using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class NewsTeamsFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_News_hTeamID",
                table: "News",
                column: "hTeamID");

            migrationBuilder.CreateIndex(
                name: "IX_News_vTeamID",
                table: "News",
                column: "vTeamID");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Teams_hTeamID",
                table: "News",
                column: "hTeamID",
                principalTable: "Teams",
                principalColumn: "TeamID");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Teams_vTeamID",
                table: "News",
                column: "vTeamID",
                principalTable: "Teams",
                principalColumn: "TeamID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_Teams_hTeamID",
                table: "News");

            migrationBuilder.DropForeignKey(
                name: "FK_News_Teams_vTeamID",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_News_hTeamID",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_News_vTeamID",
                table: "News");
        }
    }
}
