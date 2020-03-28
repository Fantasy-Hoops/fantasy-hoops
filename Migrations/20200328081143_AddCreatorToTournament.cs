using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class AddCreatorToTournament : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorID",
                table: "Tournaments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_CreatorID",
                table: "Tournaments",
                column: "CreatorID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_AspNetUsers_CreatorID",
                table: "Tournaments",
                column: "CreatorID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_AspNetUsers_CreatorID",
                table: "Tournaments");

            migrationBuilder.DropIndex(
                name: "IX_Tournaments_CreatorID",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "CreatorID",
                table: "Tournaments");
        }
    }
}
