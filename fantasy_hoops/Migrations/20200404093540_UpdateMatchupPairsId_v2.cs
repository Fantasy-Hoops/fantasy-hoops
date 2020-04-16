using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class UpdateMatchupPairsId_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TournamentMatchups",
                table: "TournamentMatchups");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TournamentMatchups",
                table: "TournamentMatchups",
                columns: new[] { "TournamentID", "FirstUserID", "SecondUserID", "ContestId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TournamentMatchups",
                table: "TournamentMatchups");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TournamentMatchups",
                table: "TournamentMatchups",
                columns: new[] { "TournamentID", "FirstUserID", "SecondUserID" });
        }
    }
}
