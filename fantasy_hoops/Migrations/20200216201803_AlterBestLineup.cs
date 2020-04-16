using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class AlterBestLineup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_BestLineup_BestLineupId_BestLineupDate",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_BestLineupId_BestLineupDate",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BestLineupDate",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BestLineupId",
                table: "Players");

            migrationBuilder.CreateTable(
                name: "PlayersBestLineups",
                columns: table => new
                {
                    PlayerID = table.Column<int>(nullable: false),
                    BestLineupID = table.Column<int>(nullable: false),
                    BestLineupDate = table.Column<DateTime>(type: "Date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayersBestLineups", x => new { x.PlayerID, x.BestLineupID });
                    table.ForeignKey(
                        name: "FK_PlayersBestLineups_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "PlayerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayersBestLineups_BestLineup_BestLineupID_BestLineupDate",
                        columns: x => new { x.BestLineupID, x.BestLineupDate },
                        principalTable: "BestLineup",
                        principalColumns: new[] { "Id", "Date" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayersBestLineups_BestLineupID_BestLineupDate",
                table: "PlayersBestLineups",
                columns: new[] { "BestLineupID", "BestLineupDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayersBestLineups");

            migrationBuilder.AddColumn<DateTime>(
                name: "BestLineupDate",
                table: "Players",
                type: "Date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BestLineupId",
                table: "Players",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_BestLineupId_BestLineupDate",
                table: "Players",
                columns: new[] { "BestLineupId", "BestLineupDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_Players_BestLineup_BestLineupId_BestLineupDate",
                table: "Players",
                columns: new[] { "BestLineupId", "BestLineupDate" },
                principalTable: "BestLineup",
                principalColumns: new[] { "Id", "Date" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
