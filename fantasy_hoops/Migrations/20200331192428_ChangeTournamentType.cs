using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class ChangeTournamentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contests_Tournaments_TournamentID",
                table: "Contests");

            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_TournamentTypes_TypeID",
                table: "Tournaments");

            migrationBuilder.DropTable(
                name: "TournamentTypes");

            migrationBuilder.DropIndex(
                name: "IX_Tournaments_TypeID",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "Contests",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "TournamentMatchups");

            migrationBuilder.RenameColumn(
                name: "TournamentID",
                table: "Contests",
                newName: "TournamentId");

            migrationBuilder.RenameIndex(
                name: "IX_Contests_TournamentID",
                table: "Contests",
                newName: "IX_Contests_TournamentId");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Tournaments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "Contests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "WinnerId",
                table: "Contests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contests_WinnerId",
                table: "Contests",
                column: "WinnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contests_Tournaments_TournamentId",
                table: "Contests",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contests_AspNetUsers_WinnerId",
                table: "Contests",
                column: "WinnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contests_Tournaments_TournamentId",
                table: "Contests");

            migrationBuilder.DropForeignKey(
                name: "FK_Contests_AspNetUsers_WinnerId",
                table: "Contests");

            migrationBuilder.DropIndex(
                name: "IX_Contests_WinnerId",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "WinnerId",
                table: "Contests");

            migrationBuilder.RenameColumn(
                name: "TournamentId",
                table: "Contests",
                newName: "TournamentID");

            migrationBuilder.RenameIndex(
                name: "IX_Contests_TournamentId",
                table: "Contests",
                newName: "IX_Contests_TournamentID");

            migrationBuilder.AddColumn<int>(
                name: "Contests",
                table: "Tournaments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "TournamentMatchups",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_TypeID",
                table: "Tournaments",
                column: "TypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Contests_Tournaments_TournamentID",
                table: "Contests",
                column: "TournamentID",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_TournamentTypes_TypeID",
                table: "Tournaments",
                column: "TypeID",
                principalTable: "TournamentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
