using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class UserFavoriteTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FavoriteTeamId",
                table: "AspNetUsers",
                column: "FavoriteTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Teams_FavoriteTeamId",
                table: "AspNetUsers",
                column: "FavoriteTeamId",
                principalTable: "Teams",
                principalColumn: "TeamID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Teams_FavoriteTeamId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FavoriteTeamId",
                table: "AspNetUsers");
        }
    }
}
