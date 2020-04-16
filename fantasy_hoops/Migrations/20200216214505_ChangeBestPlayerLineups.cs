using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class ChangeBestPlayerLineups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayersBestLineups_BestLineup_BestLineupID_BestLineupDate",
                table: "PlayersBestLineups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BestLineup",
                table: "BestLineup");

            migrationBuilder.DropColumn(
                name: "CFP",
                table: "BestLineup");

            migrationBuilder.DropColumn(
                name: "CPrice",
                table: "BestLineup");

            migrationBuilder.DropColumn(
                name: "PfFP",
                table: "BestLineup");

            migrationBuilder.DropColumn(
                name: "PfPrice",
                table: "BestLineup");

            migrationBuilder.DropColumn(
                name: "PgFP",
                table: "BestLineup");

            migrationBuilder.DropColumn(
                name: "PgPrice",
                table: "BestLineup");

            migrationBuilder.DropColumn(
                name: "SfFP",
                table: "BestLineup");

            migrationBuilder.DropColumn(
                name: "SfPrice",
                table: "BestLineup");

            migrationBuilder.DropColumn(
                name: "SgFP",
                table: "BestLineup");

            migrationBuilder.DropColumn(
                name: "SgPrice",
                table: "BestLineup");

            migrationBuilder.RenameTable(
                name: "BestLineup",
                newName: "BestLineups");

            migrationBuilder.AddColumn<double>(
                name: "FP",
                table: "PlayersBestLineups",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "PlayersBestLineups",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BestLineups",
                table: "BestLineups",
                columns: new[] { "Id", "Date" });

            migrationBuilder.AddForeignKey(
                name: "FK_PlayersBestLineups_BestLineups_BestLineupID_BestLineupDate",
                table: "PlayersBestLineups",
                columns: new[] { "BestLineupID", "BestLineupDate" },
                principalTable: "BestLineups",
                principalColumns: new[] { "Id", "Date" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayersBestLineups_BestLineups_BestLineupID_BestLineupDate",
                table: "PlayersBestLineups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BestLineups",
                table: "BestLineups");

            migrationBuilder.DropColumn(
                name: "FP",
                table: "PlayersBestLineups");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "PlayersBestLineups");

            migrationBuilder.RenameTable(
                name: "BestLineups",
                newName: "BestLineup");

            migrationBuilder.AddColumn<double>(
                name: "CFP",
                table: "BestLineup",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "CPrice",
                table: "BestLineup",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PfFP",
                table: "BestLineup",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "PfPrice",
                table: "BestLineup",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PgFP",
                table: "BestLineup",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "PgPrice",
                table: "BestLineup",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "SfFP",
                table: "BestLineup",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "SfPrice",
                table: "BestLineup",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "SgFP",
                table: "BestLineup",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "SgPrice",
                table: "BestLineup",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BestLineup",
                table: "BestLineup",
                columns: new[] { "Id", "Date" });

            migrationBuilder.AddForeignKey(
                name: "FK_PlayersBestLineups_BestLineup_BestLineupID_BestLineupDate",
                table: "PlayersBestLineups",
                columns: new[] { "BestLineupID", "BestLineupDate" },
                principalTable: "BestLineup",
                principalColumns: new[] { "Id", "Date" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
