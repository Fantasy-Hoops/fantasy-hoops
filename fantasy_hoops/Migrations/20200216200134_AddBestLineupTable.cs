using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class AddBestLineupTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BestLineupDate",
                table: "Players",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BestLineupId",
                table: "Players",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BestLineup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(type: "Date", nullable: false),
                    PgFP = table.Column<double>(nullable: false),
                    PgPrice = table.Column<int>(nullable: false),
                    SgFP = table.Column<double>(nullable: false),
                    SgPrice = table.Column<int>(nullable: false),
                    SfFP = table.Column<double>(nullable: false),
                    SfPrice = table.Column<int>(nullable: false),
                    PfFP = table.Column<double>(nullable: false),
                    PfPrice = table.Column<int>(nullable: false),
                    CFP = table.Column<double>(nullable: false),
                    CPrice = table.Column<int>(nullable: false),
                    TotalFP = table.Column<double>(nullable: false),
                    LineupPrice = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BestLineup", x => new { x.Id, x.Date });
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_BestLineup_BestLineupId_BestLineupDate",
                table: "Players");

            migrationBuilder.DropTable(
                name: "BestLineup");

            migrationBuilder.DropIndex(
                name: "IX_Players_BestLineupId_BestLineupDate",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BestLineupDate",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BestLineupId",
                table: "Players");
        }
    }
}
