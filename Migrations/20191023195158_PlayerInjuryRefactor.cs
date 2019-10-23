using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class PlayerInjuryRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Injuries_PlayerID",
                table: "Injuries");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "StatusDate",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "Injury",
                table: "Injuries",
                newName: "InjuryTitle");

            migrationBuilder.AddColumn<int>(
                name: "InjuryID",
                table: "Players",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Injuries_PlayerID",
                table: "Injuries",
                column: "PlayerID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Injuries_PlayerID",
                table: "Injuries");

            migrationBuilder.DropColumn(
                name: "InjuryID",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "InjuryTitle",
                table: "Injuries",
                newName: "Injury");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Players",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusDate",
                table: "Players",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Injuries_PlayerID",
                table: "Injuries",
                column: "PlayerID");
        }
    }
}
