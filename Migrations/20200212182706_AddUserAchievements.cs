using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class AddUserAchievements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAchievement_Achievements_AchievementID",
                table: "UserAchievement");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAchievement_AspNetUsers_UserID",
                table: "UserAchievement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAchievement",
                table: "UserAchievement");

            migrationBuilder.RenameTable(
                name: "UserAchievement",
                newName: "UserAchievements");

            migrationBuilder.RenameIndex(
                name: "IX_UserAchievement_AchievementID",
                table: "UserAchievements",
                newName: "IX_UserAchievements_AchievementID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAchievements",
                table: "UserAchievements",
                columns: new[] { "UserID", "AchievementID" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserAchievements_Achievements_AchievementID",
                table: "UserAchievements",
                column: "AchievementID",
                principalTable: "Achievements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAchievements_AspNetUsers_UserID",
                table: "UserAchievements",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAchievements_Achievements_AchievementID",
                table: "UserAchievements");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAchievements_AspNetUsers_UserID",
                table: "UserAchievements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAchievements",
                table: "UserAchievements");

            migrationBuilder.RenameTable(
                name: "UserAchievements",
                newName: "UserAchievement");

            migrationBuilder.RenameIndex(
                name: "IX_UserAchievements_AchievementID",
                table: "UserAchievement",
                newName: "IX_UserAchievement_AchievementID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAchievement",
                table: "UserAchievement",
                columns: new[] { "UserID", "AchievementID" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserAchievement_Achievements_AchievementID",
                table: "UserAchievement",
                column: "AchievementID",
                principalTable: "Achievements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAchievement_AspNetUsers_UserID",
                table: "UserAchievement",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
