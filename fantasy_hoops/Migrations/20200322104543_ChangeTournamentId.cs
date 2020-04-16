using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class ChangeTournamentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contests_Tournaments_TournamentID",
                table: "Contests");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentMatchups_Tournaments_TournamentID",
                table: "TournamentMatchups");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentUsers_Tournaments_TournamentID",
                table: "TournamentUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tournaments",
                table: "Tournaments");

            migrationBuilder.DropIndex(
                name: "IX_Contests_TournamentID",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Tournaments");

            migrationBuilder.AddColumn<string>(
                name: "Tournamenttid",
                table: "TournamentUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tid",
                table: "Tournaments",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tournamenttid",
                table: "TournamentMatchups",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tournamenttid",
                table: "Contests",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tournaments",
                table: "Tournaments",
                column: "tid");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentUsers_Tournamenttid",
                table: "TournamentUsers",
                column: "Tournamenttid");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatchups_Tournamenttid",
                table: "TournamentMatchups",
                column: "Tournamenttid");

            migrationBuilder.CreateIndex(
                name: "IX_Contests_Tournamenttid",
                table: "Contests",
                column: "Tournamenttid");

            migrationBuilder.AddForeignKey(
                name: "FK_Contests_Tournaments_Tournamenttid",
                table: "Contests",
                column: "Tournamenttid",
                principalTable: "Tournaments",
                principalColumn: "tid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentMatchups_Tournaments_Tournamenttid",
                table: "TournamentMatchups",
                column: "Tournamenttid",
                principalTable: "Tournaments",
                principalColumn: "tid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentUsers_Tournaments_Tournamenttid",
                table: "TournamentUsers",
                column: "Tournamenttid",
                principalTable: "Tournaments",
                principalColumn: "tid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contests_Tournaments_Tournamenttid",
                table: "Contests");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentMatchups_Tournaments_Tournamenttid",
                table: "TournamentMatchups");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentUsers_Tournaments_Tournamenttid",
                table: "TournamentUsers");

            migrationBuilder.DropIndex(
                name: "IX_TournamentUsers_Tournamenttid",
                table: "TournamentUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tournaments",
                table: "Tournaments");

            migrationBuilder.DropIndex(
                name: "IX_TournamentMatchups_Tournamenttid",
                table: "TournamentMatchups");

            migrationBuilder.DropIndex(
                name: "IX_Contests_Tournamenttid",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "Tournamenttid",
                table: "TournamentUsers");

            migrationBuilder.DropColumn(
                name: "tid",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "Tournamenttid",
                table: "TournamentMatchups");

            migrationBuilder.DropColumn(
                name: "Tournamenttid",
                table: "Contests");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Tournaments",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tournaments",
                table: "Tournaments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Contests_TournamentID",
                table: "Contests",
                column: "TournamentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Contests_Tournaments_TournamentID",
                table: "Contests",
                column: "TournamentID",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentMatchups_Tournaments_TournamentID",
                table: "TournamentMatchups",
                column: "TournamentID",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentUsers_Tournaments_TournamentID",
                table: "TournamentUsers",
                column: "TournamentID",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
