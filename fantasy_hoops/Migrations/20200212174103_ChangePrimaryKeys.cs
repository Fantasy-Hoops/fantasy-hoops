using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class ChangePrimaryKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAchievement_Achievements_AchievementID",
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
                name: "Id",
                table: "UserAchievement");

            migrationBuilder.DropColumn(
                name: "AchievementID",
                table: "UserAchievement");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Achievements");

            migrationBuilder.AddColumn<int>(
                name: "Uaid",
                table: "UserAchievement",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "AID",
                table: "UserAchievement",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AchievementAid",
                table: "UserAchievement",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UID",
                table: "UserAchievement",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Aid",
                table: "Achievements",
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

            migrationBuilder.AddForeignKey(
                name: "FK_UserAchievement_Achievements_AchievementAid",
                table: "UserAchievement",
                column: "AchievementAid",
                principalTable: "Achievements",
                principalColumn: "Aid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAchievement_Achievements_AchievementAid",
                table: "UserAchievement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAchievement",
                table: "UserAchievement");

            migrationBuilder.DropIndex(
                name: "IX_UserAchievement_AchievementAid",
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

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "UserAchievement",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AchievementID",
                table: "UserAchievement",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Achievements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAchievement",
                table: "UserAchievement",
                column: "Id");

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
        }
    }
}
