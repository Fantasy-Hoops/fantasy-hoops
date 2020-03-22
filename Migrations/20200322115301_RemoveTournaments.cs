using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class RemoveTournaments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TournamentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tournaments",
                columns: table => new
                {
                    tid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Contests = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DroppedContests = table.Column<int>(type: "int", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Entrants = table.Column<int>(type: "int", nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TypeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.tid);
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContestStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TournamentID = table.Column<int>(type: "int", nullable: false),
                    Tournamenttid = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contests_Tournaments_Tournamenttid",
                        column: x => x.Tournamenttid,
                        principalTable: "Tournaments",
                        principalColumn: "tid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentUsers",
                columns: table => new
                {
                    TournamentID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Losses = table.Column<int>(type: "int", nullable: false),
                    Ties = table.Column<int>(type: "int", nullable: false),
                    Tournamenttid = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Wins = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentUsers", x => new { x.TournamentID, x.UserID });
                    table.ForeignKey(
                        name: "FK_TournamentUsers_Tournaments_Tournamenttid",
                        column: x => x.Tournamenttid,
                        principalTable: "Tournaments",
                        principalColumn: "tid",
                        onDelete: ReferentialAction.Restrict);
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
                    TournamentID = table.Column<int>(type: "int", nullable: false),
                    FirstUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SecondUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContestId = table.Column<int>(type: "int", nullable: true),
                    FirstUserScore = table.Column<double>(type: "float", nullable: false),
                    IsFinished = table.Column<bool>(type: "bit", nullable: false),
                    SecondUserScore = table.Column<double>(type: "float", nullable: false),
                    Tournamenttid = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TournamentMatchups_Tournaments_Tournamenttid",
                        column: x => x.Tournamenttid,
                        principalTable: "Tournaments",
                        principalColumn: "tid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contests_Tournamenttid",
                table: "Contests",
                column: "Tournamenttid");

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
                name: "IX_TournamentMatchups_Tournamenttid",
                table: "TournamentMatchups",
                column: "Tournamenttid");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_TypeID",
                table: "Tournaments",
                column: "TypeID");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentUsers_Tournamenttid",
                table: "TournamentUsers",
                column: "Tournamenttid");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentUsers_UserID",
                table: "TournamentUsers",
                column: "UserID");
        }
    }
}
