using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class AlterBestLineupKeyChangeBack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayersBestLineups_BestLineups_BID",
                table: "PlayersBestLineups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayersBestLineups",
                table: "PlayersBestLineups");

            migrationBuilder.DropIndex(
                name: "IX_PlayersBestLineups_BID",
                table: "PlayersBestLineups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BestLineups",
                table: "BestLineups");

            migrationBuilder.DropColumn(
                name: "BID",
                table: "PlayersBestLineups");

            migrationBuilder.DropColumn(
                name: "BID",
                table: "BestLineups");

            migrationBuilder.AddColumn<int>(
                name: "BestLineupID",
                table: "PlayersBestLineups",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "BestLineups",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayersBestLineups",
                table: "PlayersBestLineups",
                columns: new[] { "PlayerID", "BestLineupID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BestLineups",
                table: "BestLineups",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PlayersBestLineups_BestLineupID",
                table: "PlayersBestLineups",
                column: "BestLineupID");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayersBestLineups_BestLineups_BestLineupID",
                table: "PlayersBestLineups",
                column: "BestLineupID",
                principalTable: "BestLineups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayersBestLineups_BestLineups_BestLineupID",
                table: "PlayersBestLineups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayersBestLineups",
                table: "PlayersBestLineups");

            migrationBuilder.DropIndex(
                name: "IX_PlayersBestLineups_BestLineupID",
                table: "PlayersBestLineups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BestLineups",
                table: "BestLineups");

            migrationBuilder.DropColumn(
                name: "BestLineupID",
                table: "PlayersBestLineups");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BestLineups");

            migrationBuilder.AddColumn<int>(
                name: "BID",
                table: "PlayersBestLineups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BID",
                table: "BestLineups",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayersBestLineups",
                table: "PlayersBestLineups",
                columns: new[] { "PlayerID", "BID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BestLineups",
                table: "BestLineups",
                column: "BID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayersBestLineups_BID",
                table: "PlayersBestLineups",
                column: "BID");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayersBestLineups_BestLineups_BID",
                table: "PlayersBestLineups",
                column: "BID",
                principalTable: "BestLineups",
                principalColumn: "BID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
