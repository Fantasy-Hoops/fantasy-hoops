using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fantasy_hoops.Migrations
{
    public partial class DateTimeToDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Games",
                type: "Date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Games",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "Date",
                oldNullable: true);
        }
    }
}
