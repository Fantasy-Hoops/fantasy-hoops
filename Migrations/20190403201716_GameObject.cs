using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class GameObject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameID",
                table: "Stats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: true),
                    HomeTeamID = table.Column<int>(nullable: false),
                    HomeScore = table.Column<int>(nullable: false),
                    AwayTeamID = table.Column<int>(nullable: false),
                    AwayScore = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameID);
                    table.ForeignKey(
                        name: "FK_Games_Teams_AwayTeamID",
                        column: x => x.AwayTeamID,
                        principalTable: "Teams",
                        principalColumn: "TeamID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_Teams_HomeTeamID",
                        column: x => x.HomeTeamID,
                        principalTable: "Teams",
                        principalColumn: "TeamID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stats_GameID",
                table: "Stats",
                column: "GameID");

            migrationBuilder.CreateIndex(
                name: "IX_Games_AwayTeamID",
                table: "Games",
                column: "AwayTeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Games_HomeTeamID",
                table: "Games",
                column: "HomeTeamID");

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

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Stats_GameID",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "GameID",
                table: "Stats");
        }
    }
}
