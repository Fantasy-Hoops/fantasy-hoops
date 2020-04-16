using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class AlterPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAchievement_Achievements_AchievementAid",
                table: "UserAchievement");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAchievement_AspNetUsers_UserId",
                table: "UserAchievement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAchievement",
                table: "UserAchievement");

            migrationBuilder.DropIndex(
                name: "IX_UserAchievement_AchievementAid",
                table: "UserAchievement");

            migrationBuilder.DropIndex(
                name: "IX_UserAchievement_UserId",
                table: "UserAchievement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Achievements",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "Uaid",
                table: "UserAchievement");

            migrationBuilder.DropColumn(
                name: "AID",
                table: "UserAchievement");

            migrationBuilder.DropColumn(
                name: "AchievementAid",
                table: "UserAchievement");

            migrationBuilder.DropColumn(
                name: "UID",
                table: "UserAchievement");

            migrationBuilder.DropColumn(
                name: "Aid",
                table: "Achievements");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserAchievement",
                newName: "UserID");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "UserAchievement",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AchievementID",
                table: "UserAchievement",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Achievements",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAchievement",
                table: "UserAchievement",
                columns: new[] { "UserID", "AchievementID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Achievements",
                table: "Achievements",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievement_AchievementID",
                table: "UserAchievement",
                column: "AchievementID");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "IX_UserAchievement_AchievementID",
                table: "UserAchievement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Achievements",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "AchievementID",
                table: "UserAchievement");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Achievements");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "UserAchievement",
                newName: "UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserAchievement",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "Uaid",
                table: "UserAchievement",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "AID",
                table: "UserAchievement",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AchievementAid",
                table: "UserAchievement",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UID",
                table: "UserAchievement",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Aid",
                table: "Achievements",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAchievement",
                table: "UserAchievement",
                column: "Uaid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Achievements",
                table: "Achievements",
                column: "Aid");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievement_AchievementAid",
                table: "UserAchievement",
                column: "AchievementAid");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievement_UserId",
                table: "UserAchievement",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAchievement_Achievements_AchievementAid",
                table: "UserAchievement",
                column: "AchievementAid",
                principalTable: "Achievements",
                principalColumn: "Aid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAchievement_AspNetUsers_UserId",
                table: "UserAchievement",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
