using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class RevertTournaments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TournamentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tournaments",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    TypeID = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ImageURL = table.Column<string>(nullable: true),
                    Entrants = table.Column<int>(nullable: false),
                    Contests = table.Column<int>(nullable: false),
                    DroppedContests = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tournaments_TournamentTypes_TypeID",
                        column: x => x.TypeID,
                        principalTable: "TournamentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContestStart = table.Column<DateTime>(nullable: false),
                    TournamentID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contests_Tournaments_TournamentID",
                        column: x => x.TournamentID,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TournamentUsers",
                columns: table => new
                {
                    UserID = table.Column<string>(nullable: false),
                    TournamentID = table.Column<string>(nullable: false),
                    Wins = table.Column<int>(nullable: false),
                    Losses = table.Column<int>(nullable: false),
                    Ties = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentUsers", x => new { x.TournamentID, x.UserID });
                    table.ForeignKey(
                        name: "FK_TournamentUsers_Tournaments_TournamentID",
                        column: x => x.TournamentID,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TournamentUsers_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentMatchups",
                columns: table => new
                {
                    TournamentID = table.Column<string>(nullable: false),
                    FirstUserID = table.Column<string>(nullable: false),
                    SecondUserID = table.Column<string>(nullable: false),
                    FirstUserScore = table.Column<double>(nullable: false),
                    SecondUserScore = table.Column<double>(nullable: false),
                    IsFinished = table.Column<bool>(nullable: false),
                    ContestId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentMatchups", x => new { x.TournamentID, x.FirstUserID, x.SecondUserID });
                    table.ForeignKey(
                        name: "FK_TournamentMatchups_Contests_ContestId",
                        column: x => x.ContestId,
                        principalTable: "Contests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TournamentMatchups_AspNetUsers_FirstUserID",
                        column: x => x.FirstUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TournamentMatchups_AspNetUsers_SecondUserID",
                        column: x => x.SecondUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TournamentMatchups_Tournaments_TournamentID",
                        column: x => x.TournamentID,
                        principalTable: "Tournaments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contests_TournamentID",
                table: "Contests",
                column: "TournamentID");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatchups_ContestId",
                table: "TournamentMatchups",
                column: "ContestId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatchups_FirstUserID",
                table: "TournamentMatchups",
                column: "FirstUserID");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatchups_SecondUserID",
                table: "TournamentMatchups",
                column: "SecondUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_TypeID",
                table: "Tournaments",
                column: "TypeID");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentUsers_UserID",
                table: "TournamentUsers",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TournamentMatchups");

            migrationBuilder.DropTable(
                name: "TournamentUsers");

            migrationBuilder.DropTable(
                name: "Contests");

            migrationBuilder.DropTable(
                name: "Tournaments");

            migrationBuilder.DropTable(
                name: "TournamentTypes");
        }
    }
}
