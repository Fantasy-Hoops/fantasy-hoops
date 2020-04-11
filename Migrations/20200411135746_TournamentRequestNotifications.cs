using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class TournamentRequestNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiverID",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestType",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderID",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TournamentId",
                table: "Notifications",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TournamentInvite",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvitedUserID = table.Column<string>(nullable: true),
                    TournamentID = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentInvite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentInvite_AspNetUsers_InvitedUserID",
                        column: x => x.InvitedUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TournamentInvite_Tournaments_TournamentID",
                        column: x => x.TournamentID,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TournamentInvite_InvitedUserID",
                table: "TournamentInvite",
                column: "InvitedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentInvite_TournamentID",
                table: "TournamentInvite",
                column: "TournamentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TournamentInvite");

            migrationBuilder.DropColumn(
                name: "ReceiverID",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RequestType",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SenderID",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TournamentId",
                table: "Notifications");
        }
    }
}
