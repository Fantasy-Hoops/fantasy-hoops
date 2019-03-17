using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class UserLineup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLineups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<string>(nullable: true),
                    PgID = table.Column<int>(nullable: false),
                    SgID = table.Column<int>(nullable: false),
                    SfID = table.Column<int>(nullable: false),
                    PfID = table.Column<int>(nullable: false),
                    CID = table.Column<int>(nullable: false),
                    FP = table.Column<double>(nullable: false),
                    IsCalculated = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLineups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserLineups_Players_CID",
                        column: x => x.CID,
                        principalTable: "Players",
                        principalColumn: "PlayerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLineups_Players_PfID",
                        column: x => x.PfID,
                        principalTable: "Players",
                        principalColumn: "PlayerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLineups_Players_PgID",
                        column: x => x.PgID,
                        principalTable: "Players",
                        principalColumn: "PlayerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLineups_Players_SfID",
                        column: x => x.SfID,
                        principalTable: "Players",
                        principalColumn: "PlayerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLineups_Players_SgID",
                        column: x => x.SgID,
                        principalTable: "Players",
                        principalColumn: "PlayerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLineups_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLineups_CID",
                table: "UserLineups",
                column: "CID");

            migrationBuilder.CreateIndex(
                name: "IX_UserLineups_PfID",
                table: "UserLineups",
                column: "PfID");

            migrationBuilder.CreateIndex(
                name: "IX_UserLineups_PgID",
                table: "UserLineups",
                column: "PgID");

            migrationBuilder.CreateIndex(
                name: "IX_UserLineups_SfID",
                table: "UserLineups",
                column: "SfID");

            migrationBuilder.CreateIndex(
                name: "IX_UserLineups_SgID",
                table: "UserLineups",
                column: "SgID");

            migrationBuilder.CreateIndex(
                name: "IX_UserLineups_UserID",
                table: "UserLineups",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLineups");
        }
    }
}
