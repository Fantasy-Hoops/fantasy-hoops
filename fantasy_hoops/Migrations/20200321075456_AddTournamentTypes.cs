using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class AddTournamentTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Tournaments");

            migrationBuilder.AddColumn<int>(
                name: "TypeID",
                table: "Tournaments",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_TypeID",
                table: "Tournaments",
                column: "TypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_TournamentTypes_TypeID",
                table: "Tournaments",
                column: "TypeID",
                principalTable: "TournamentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_TournamentTypes_TypeID",
                table: "Tournaments");

            migrationBuilder.DropTable(
                name: "TournamentTypes");

            migrationBuilder.DropIndex(
                name: "IX_Tournaments_TypeID",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "TypeID",
                table: "Tournaments");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Tournaments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
