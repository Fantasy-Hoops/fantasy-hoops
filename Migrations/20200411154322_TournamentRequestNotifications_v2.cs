using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class TournamentRequestNotifications_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TournamentInvite_AspNetUsers_InvitedUserID",
                table: "TournamentInvite");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentInvite_Tournaments_TournamentID",
                table: "TournamentInvite");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TournamentInvite",
                table: "TournamentInvite");

            migrationBuilder.RenameTable(
                name: "TournamentInvite",
                newName: "TournamentInvites");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentInvite_TournamentID",
                table: "TournamentInvites",
                newName: "IX_TournamentInvites_TournamentID");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentInvite_InvitedUserID",
                table: "TournamentInvites",
                newName: "IX_TournamentInvites_InvitedUserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TournamentInvites",
                table: "TournamentInvites",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentInvites_AspNetUsers_InvitedUserID",
                table: "TournamentInvites",
                column: "InvitedUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentInvites_Tournaments_TournamentID",
                table: "TournamentInvites",
                column: "TournamentID",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TournamentInvites_AspNetUsers_InvitedUserID",
                table: "TournamentInvites");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentInvites_Tournaments_TournamentID",
                table: "TournamentInvites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TournamentInvites",
                table: "TournamentInvites");

            migrationBuilder.RenameTable(
                name: "TournamentInvites",
                newName: "TournamentInvite");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentInvites_TournamentID",
                table: "TournamentInvite",
                newName: "IX_TournamentInvite_TournamentID");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentInvites_InvitedUserID",
                table: "TournamentInvite",
                newName: "IX_TournamentInvite_InvitedUserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TournamentInvite",
                table: "TournamentInvite",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentInvite_AspNetUsers_InvitedUserID",
                table: "TournamentInvite",
                column: "InvitedUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentInvite_Tournaments_TournamentID",
                table: "TournamentInvite",
                column: "TournamentID",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
