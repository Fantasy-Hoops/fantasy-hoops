using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class UpdatePushSubscribtionModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PushSubscriptions",
                newName: "UserID");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "PushSubscriptions",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_PushSubscriptions_UserID",
                table: "PushSubscriptions",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_PushSubscriptions_AspNetUsers_UserID",
                table: "PushSubscriptions",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PushSubscriptions_AspNetUsers_UserID",
                table: "PushSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_PushSubscriptions_UserID",
                table: "PushSubscriptions");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "PushSubscriptions",
                newName: "UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PushSubscriptions",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
