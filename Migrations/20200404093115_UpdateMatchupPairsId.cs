using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class UpdateMatchupPairsId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TournamentMatchups_Contests_ContestId",
                table: "TournamentMatchups");

            migrationBuilder.AlterColumn<int>(
                name: "ContestId",
                table: "TournamentMatchups",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentMatchups_Contests_ContestId",
                table: "TournamentMatchups",
                column: "ContestId",
                principalTable: "Contests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TournamentMatchups_Contests_ContestId",
                table: "TournamentMatchups");

            migrationBuilder.AlterColumn<int>(
                name: "ContestId",
                table: "TournamentMatchups",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentMatchups_Contests_ContestId",
                table: "TournamentMatchups",
                column: "ContestId",
                principalTable: "Contests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
